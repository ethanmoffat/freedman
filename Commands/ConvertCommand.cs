using AutomaticTypeMapper;
using Azure;
using DSharpPlus.Entities;
using freedman.Converters;
using freedman.Parser;
using freedman.Precision;
using freedman.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace freedman.Commands
{
    [AutoMappedType]
    public class ConvertCommand : ICommand
    {
        private readonly IPrecisionCacheRepository _precisionCacheRepository;
        private readonly IPrecisionTableProvider _precisionTableProvider;
        private readonly IUnitParser _parser;
        private readonly IEnumerable<IUnitConverter> _converters;

        public string Name => "convert";

        public ConvertCommand(IPrecisionCacheRepository precisionCacheRepository,
                              IPrecisionTableProvider precisionTableProvider,
                              IUnitParser parser,
                              IEnumerable<IUnitConverter> converters)
        {
            _precisionCacheRepository = precisionCacheRepository;
            _precisionTableProvider = precisionTableProvider;
            _parser = parser;
            _converters = converters;
        }

        public bool IsHandlerFor(string command) => string.Equals(Name, command, StringComparison.OrdinalIgnoreCase);

        public async Task HandleCommandAsync(string[] messageParts, DiscordMessage message, DiscordGuild guild)
        {
            if (messageParts.Length <= 1)
                return;

            if (!_precisionCacheRepository.PrecisionCache.ContainsKey(guild.Id))
            {
                try
                {
                    PrecisionTableRecord precisionRecord = await _precisionTableProvider.TableClient.GetEntityAsync<PrecisionTableRecord>($"{guild.Id}", "precision");
                    _precisionCacheRepository.PrecisionCache[guild.Id] = precisionRecord.Precision;
                }
                catch (RequestFailedException)
                {
                    // entity doesn't exist
                    _precisionCacheRepository.PrecisionCache[guild.Id] = 4;
                }
            }

            IUnit unit, target;
            try
            {
                (unit, target) = _parser.Parse(messageParts);
            }
            catch (ArgumentException ae)
            {
                await message.RespondAsync("Error: " + ae.Message);
                return;
            }

            IUnit converted;
            try
            {
                converted = await Convert(unit, target);
            }
            catch (Exception ex) when (ex is RequestFailedException || ex is ArgumentException)
            {
                await message.RespondAsync("Error: " + ex.Message);
                return;
            }
            catch (Exception ex)
            {
                await message.RespondAsync("Unspecified error occurred: " + ex.Message);
                return;
            }

            var extra = unit.Value.ToString().Split(".")[0] == "69"
                ? " (nice)"
                : string.Empty;

            var extra2 = converted?.Value.ToString().Split(".")[0] == "69"
                ? " (nice)"
                : string.Empty;

            var response = converted == null
                ? $"I don't know how to convert {unit.Units} into {target.Units}"
                : $"{unit.Value}{extra} {unit.Units} is {Math.Round(converted.Value, _precisionCacheRepository.PrecisionCache[guild.Id])}{extra2} {converted.Units}";

            await message.RespondAsync(response);
        }

        private async Task<IUnit> Convert(IUnit unit, IUnit target)
        {
            var converter = _converters.SingleOrDefault(x => x.IsConverterFor(unit));
            if (converter != null)
            {
                var targetConverter = _converters.SingleOrDefault(x => x.IsConverterFor(target));
                if (targetConverter != null)
                {
                    return await targetConverter.FromSIUnitAsync(await converter.ToSIUnitAsync(unit));
                }
            }

            return null;
        }
    }

    [AutoMappedType]
    public class ConvertCommandHelpInfo : ICommandHelpInfo
    {
        public string Name => "convert";

        public string GetUsage() => $"{Name} <value> <units> [to <units>]";

        public string GetDescription() => "Convert some unit to a different (compatible) unit. Target unit is optional.";
    }
}
