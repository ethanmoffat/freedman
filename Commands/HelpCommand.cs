using AutomaticTypeMapper;
using DSharpPlus.Entities;
using freedman.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace freedman.Commands
{
    [AutoMappedType]
    public class HelpCommand : ICommand
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IEnumerable<ICommandHelpInfo> _commandHelpInfo;

        public string Name => "help";

        public HelpCommand(IConfigurationProvider configurationProvider,
                           IEnumerable<ICommandHelpInfo> commandHelpInfo)
        {
            _configurationProvider = configurationProvider;
            _commandHelpInfo = commandHelpInfo;
        }

        public bool IsHandlerFor(string command) => string.Equals(Name, command, StringComparison.OrdinalIgnoreCase);

        public string GetUsage() => $"{Name} [commandName]";

        public async Task HandleCommandAsync(string[] input, DiscordMessage message, DiscordGuild guild)
        {
            var prefix = _configurationProvider.Configuration["freedman-command-prefix"];

            if (input.Length == 1)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Help Command",
                    Description = "Available commands:\n" + string.Join('\n', _commandHelpInfo.Select(x => $"`{x.Name}`")) + $"\nType `{prefix} help <command>` for details on a command",
                };
                await message.RespondAsync(embed.Build());
            }
            else
            {
                var command = _commandHelpInfo.SingleOrDefault(c => string.Equals(c.Name, input[1], StringComparison.OrdinalIgnoreCase));
                if (command != null)
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = $"Help Command - {input[1]}",
                        Description = $"{command.GetDescription()}\nUsage: `{prefix} {command.GetUsage()}`",
                    };
                    await message.RespondAsync(embed.Build());
                }
            }
        }
    }

    [AutoMappedType]
    public class HelpCommandHelpInfo : ICommandHelpInfo
    {
        public string Name => "help";

        public string GetUsage() => $"{Name} [commandName]";

        public string GetDescription() => "Show a help message for Freedman, or for a specific command";
    }
}
