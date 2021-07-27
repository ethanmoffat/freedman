using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using DSharpPlus;
using DSharpPlus.EventArgs;
using freedman.Converters;
using freedman.Converters.Length;
using freedman.Parser;
using freedman.Unit;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using AutomaticTypeMapper;
using System.Reflection;

namespace freedman
{
    public class Program
    {
        private static readonly string CurrencyConvertUrl = "https://free.currconv.com/api/v7/convert?q={0}&compact=ultra&apiKey={1}";
        private static HttpClient _httpClient;

        private static double USD_CAD = 0.0;
        private static DateTime _lastFetchUsdCad;

        private static double CAD_USD = 0.0;
        private static DateTime _lastFetchCadUsd;

        private static IConfigurationRoot _config;
        private static TableClient _tableClient;

        private static IDictionary<ulong, int> _precision;

        // todo: move stuff out of main and use proper injection
        private static ITypeRegistry _registry;

        public static async Task Main(string[] args)
        {
            _registry = new UnityRegistry(Assembly.GetExecutingAssembly().FullName);
            _registry.RegisterDiscoveredTypes();

            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(Environment.GetEnvironmentVariable("ConnectionString"));
            _config = builder.Build();

            _httpClient = new HttpClient();

            _lastFetchUsdCad = DateTime.Now;
            _lastFetchCadUsd = DateTime.Now;

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
                _httpClient?.Dispose();
                await discordClient?.DisconnectAsync();
                discordClient?.Dispose();
                _registry?.Dispose();
            }
        }

        private static async Task OnMessageCreated(MessageCreateEventArgs e)
        {
            if (e.Author.IsBot)
                return;

            List<string> messageParts;

            if (e.Message.Content.StartsWith("!precision ", StringComparison.OrdinalIgnoreCase))
            {
                messageParts = e.Message.Content.Split(" ").ToList();
                if (messageParts.Count <= 1)
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
            else if (e.Message.Content.Equals("!help", StringComparison.OrdinalIgnoreCase))
            {
                await e.Message.RespondAsync("Use `!convert {value} {units}` to convert units or `!precision {value}` to set precision of values");
                return;
            }
            else if (!e.Message.Content.StartsWith("!convert ", StringComparison.OrdinalIgnoreCase))
                return;

            messageParts = e.Message.Content.Split(" ").ToList();
            if (messageParts.Count <= 1)
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
                (unit, target) = parser.Parse(e.Message.Content);
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
            catch (RequestFailedException rfe)
            {
                await e.Message.RespondAsync(rfe.Message);
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

        // todo: return type is only Length right now, need to use correct types
        private static async Task<IUnit> Convert(IUnit unit, IUnit target)
        {
            var converters = _registry.ResolveAll<IUnitConverter>();

            var converter = converters.SingleOrDefault(x => x.IsConverterFor(unit));
            if (converter != null)
            {
                var targetConverter = converters.SingleOrDefault(x => x.IsConverterFor(target));
                if (targetConverter != null)
                {
                    return targetConverter.FromSIUnit(converter.ToSIUnit(unit));
                }
            }

            var quantity = unit.Value;
            // machine learning is just if statements
            switch (unit.Units.ToLowerInvariant())
            {
                case "c":
                case "celsius":
                case "degrees c":
                case "degrees celsius":
                    return new Length(quantity * 1.8 + 32.0, "farenheit");

                case "f":
                case "farenheit":
                case "degrees f":
                case "degrees farenheit":
                    return new Length((quantity - 32.0) / 1.8, "celsius");

                case "gal":
                case "gallon":
                case "gallons":
                    return new Length((quantity * 3.78541178), "liters");

                case "liter":
                case "litre":
                case "liters":
                case "litres":
                    return new Length((quantity / 3.78541178), "gallons");

                case "usd":
                    {
                        try
                        {
                            if ((DateTime.Now - _lastFetchUsdCad).TotalHours > 1 || USD_CAD == 0.0)
                            {
                                using var response = await _httpClient.GetAsync(string.Format(CurrencyConvertUrl, "USD_CAD", _config["currency-api-key"]));
                                if (!response.IsSuccessStatusCode)
                                    throw new RequestFailedException($"Unable to contact currency conversion API (code {response.StatusCode})\nService status: https://www.currencyconverterapi.com/server-status");
                                var json = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), new { USD_CAD = 0.0 });
                                USD_CAD = json.USD_CAD;
                                _lastFetchUsdCad = DateTime.Now;
                            }

                            return new Length(quantity * USD_CAD, "CAD");
                        }
                        catch (RequestFailedException)
                        {
                            throw;
                        }
                        catch
                        {
                            break;
                        }
                    }

                case "cad":
                    {
                        try
                        {
                            if ((DateTime.Now - _lastFetchCadUsd).TotalHours > 1 || CAD_USD == 0.0)
                            {
                                using var response = await _httpClient.GetAsync(string.Format(CurrencyConvertUrl, "CAD_USD", _config["currency-api-key"]));
                                if (!response.IsSuccessStatusCode)
                                    throw new RequestFailedException($"Unable to contact currency conversion API (code {response.StatusCode})\nService status: https://www.currencyconverterapi.com/server-status");
                                var json = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), new { CAD_USD = 0.0 });
                                CAD_USD = json.CAD_USD;
                                _lastFetchCadUsd = DateTime.Now;
                            }

                            return new Length(quantity * CAD_USD, "USD");
                        }
                        catch (RequestFailedException)
                        {
                            throw;
                        }
                        catch
                        {
                            break;
                        }
                    }

                case "#":
                case "lbs":
                case "lb":
                case "pound":
                case "pounds":
                    return new Length(quantity / 2.20462262, "kilograms");

                case "kg":
                case "kilogram":
                case "kilograms":
                    return new Length(quantity * 2.20462262, "pounds");

                case "washroom":
                    return new Length(quantity, "bathroom");
                case "bathroom":
                    return new Length(quantity, "washroom");
            }

            return null;
        }
    }
}
