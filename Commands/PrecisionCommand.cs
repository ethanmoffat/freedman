using AutomaticTypeMapper;
using Azure;
using Azure.Data.Tables;
using DSharpPlus.Entities;
using freedman.Precision;
using System;
using System.Threading.Tasks;

namespace freedman.Commands
{
    [AutoMappedType]
    public class PrecisionCommand : ICommand
    {
        private readonly IPrecisionTableProvider _precisionTableProvider;
        private readonly IPrecisionCacheRepository _precisionCacheRepository;

        public string Name => "precision";

        public PrecisionCommand(IPrecisionTableProvider precisionTableProvider,
                                IPrecisionCacheRepository precisionCacheRepository)
        {
            _precisionTableProvider = precisionTableProvider;
            _precisionCacheRepository = precisionCacheRepository;
        }

        public bool IsHandlerFor(string command) => string.Equals(Name, command, StringComparison.OrdinalIgnoreCase);

        public string GetUsage() => $"{Name} <precision>";

        public async Task HandleCommandAsync(string[] messageParts, DiscordMessage message, DiscordGuild guild)
        {
            if (messageParts.Length <= 1)
                return;

            if (!int.TryParse(messageParts[1], out var precision) || precision < 0 || precision > 10)
            {
                await message.RespondAsync("Precision must be an integer between 0 and 10");
                return;
            }

            var tableClient = _precisionTableProvider.TableClient;

            PrecisionTableRecord precisionRecord;
            try
            {
                precisionRecord = await tableClient.GetEntityAsync<PrecisionTableRecord>($"{guild.Id}", "precision");
                precisionRecord.Precision = precision;
                await tableClient.UpdateEntityAsync(precisionRecord, ETag.All, TableUpdateMode.Replace);
            }
            catch (RequestFailedException)
            {
                // entity doesn't exist
                precisionRecord = new PrecisionTableRecord
                {
                    ETag = ETag.All,
                    PartitionKey = $"{guild.Id}",
                    Timestamp = DateTime.Now,
                    Precision = precision
                };
                await tableClient.AddEntityAsync(precisionRecord);
            }

            _precisionCacheRepository.PrecisionCache[guild.Id] = precision;

            await message.RespondAsync($"Precision for {guild.Name} set to {precision}");

            return;
        }
    }

    [AutoMappedType]
    public class PrecisionCommandHelpInfo : ICommandHelpInfo
    {
        public string Name => "precision";

        public string GetUsage() => $"{Name} <precision>";

        public string GetDescription() => "Set the precision for this server to <precision>";
    }
}
