using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using DSharpPlus;
using DSharpPlus.EventArgs;
using freedman.Converters;
using freedman.Parser;
using freedman.Unit;
using AutomaticTypeMapper;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace freedman
{
    public class Program
    {
        private static IConfigurationRoot _config;
        private static TableClient _tableClient;

        private static IDictionary<ulong, int> _precision;

        // todo: move stuff out of main and use proper injection
        private static ITypeRegistry _registry;

        public static async Task Main(string[] args)
        {
            _registry = new UnityRegistry(Assembly.GetExecutingAssembly().FullName);
            _registry.RegisterDiscoveredTypes();

            _config = _registry.Resolve<Configuration.IConfigurationProvider>().Configuration;

            _precision = new Dictionary<ulong, int>();

            DiscordClient discordClient = null;
            try
            {
                _tableClient = new TableClient(_config["freedman-storage"], "freedmanprecision");

                discordClient = new DiscordClient(new DiscordConfiguration
                {
                    Token = _config["freeman-token"],
                    TokenType = TokenType.Bot
                });

                discordClient.MessageCreated += OnMessageCreated;

                await discordClient.ConnectAsync();
                await Task.Delay(-1);
            }
            finally
            {
                await discordClient?.DisconnectAsync();
                discordClient?.Dispose();
                _registry?.Dispose();
            }
        }

        private static async Task OnMessageCreated(MessageCreateEventArgs e)
        {
            if (e.Author.IsBot)
                return;

            var messageParts = e.Message.Content.Split(new[] { " ", "\t", "\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (messageParts[0].Equals("!precision", StringComparison.OrdinalIgnoreCase))
            {
                if (messageParts.Length <= 1)
                    return;

                if (!int.TryParse(messageParts[1], out var precision) || precision < 0 || precision > 10)
                {
                    await e.Message.RespondAsync("Precision must be an integer between 0 and 10");
                    return;
                }

                PrecisionTableRecord precisionRecord;
                try
                {
                    precisionRecord = await _tableClient.GetEntityAsync<PrecisionTableRecord>($"{e.Guild.Id}", "precision");
                    precisionRecord.Precision = precision;
                    await _tableClient.UpdateEntityAsync(precisionRecord, ETag.All, TableUpdateMode.Replace);
                }
                catch (RequestFailedException)
                {
                    // entity doesn't exist
                    precisionRecord = new PrecisionTableRecord
                    {
                        ETag = ETag.All,
                        PartitionKey = $"{e.Guild.Id}",
                        Timestamp = DateTime.Now,
                        Precision = precision
                    };
                    await _tableClient.AddEntityAsync(precisionRecord);
                }

                _precision[e.Guild.Id] = precision;

                await e.Message.RespondAsync($"Precision for {e.Guild.Name} set to {precision}");
                return;
            }
            else if (messageParts[0].Equals("!help", StringComparison.OrdinalIgnoreCase))
            {
                await e.Message.RespondAsync("Use `!convert {value} {units} [to {units}]` to convert units or `!precision {value}` to set precision of values");
                return;
            }
            else if (!messageParts[0].Equals("!convert", StringComparison.OrdinalIgnoreCase) || messageParts.Length <= 1)
                return;

            if (!_precision.ContainsKey(e.Guild.Id))
            {
                try
                {
                    PrecisionTableRecord precisionRecord = await _tableClient.GetEntityAsync<PrecisionTableRecord>($"{e.Guild.Id}", "precision");
                    _precision[e.Guild.Id] = precisionRecord.Precision;
                }
                catch (RequestFailedException)
                {
                    // entity doesn't exist
                    _precision[e.Guild.Id] = 4;
                }
            }

            IUnit unit, target;
            try
            {
                var parser = _registry.Resolve<IUnitParser>();
                (unit, target) = parser.Parse(messageParts);
            }
            catch (ArgumentException ae)
            {
                await e.Message.RespondAsync("Error: " + ae.Message);
                return;
            }

            IUnit converted;
            try
            {
                converted = await Convert(unit, target);
            }
            catch (Exception ex) when (ex is RequestFailedException || ex is ArgumentException)
            {
                await e.Message.RespondAsync("Error: " + ex.Message);
                return;
            }
            catch (Exception ex)
            {
                await e.Message.RespondAsync("Unspecified error occurred: " + ex.Message);
                return;
            }

            var extra = unit.Value.ToString().Split(".")[0] == "69"
                ? " (nice)"
                : string.Empty;

            var extra2 = converted?.Value.ToString().Split(".")[0] == "69"
                ? " (nice)"
                : string.Empty;

            var message = converted == null
                ? $"I don't know how to convert {unit.Units} into {target.Units}"
                : $"{unit.Value}{extra} {unit.Units} is {Math.Round(converted.Value, _precision[e.Guild.Id])}{extra2} {converted.Units}";

            await e.Message.RespondAsync(message);
        }

        private static async Task<IUnit> Convert(IUnit unit, IUnit target)
        {
            var converters = _registry.ResolveAll<IUnitConverter>();

            var converter = converters.SingleOrDefault(x => x.IsConverterFor(unit));
            if (converter != null)
            {
                var targetConverter = converters.SingleOrDefault(x => x.IsConverterFor(target));
                if (targetConverter != null)
                {
                    return await targetConverter.FromSIUnitAsync(await converter.ToSIUnitAsync(unit));
                }
            }

            return null;
        }
    }
}
