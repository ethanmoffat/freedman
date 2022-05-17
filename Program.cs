using AutomaticTypeMapper;
using DSharpPlus;
using DSharpPlus.EventArgs;
using freedman.Commands;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace freedman
{
    public class Program
    {
        private static ITypeRegistry _registry;

        public static async Task Main(string[] args)
        {
            _registry = new UnityRegistry(Assembly.GetExecutingAssembly().FullName);
            _registry.RegisterDiscoveredTypes();

            var config = _registry.Resolve<Configuration.IConfigurationProvider>().Configuration;

            DiscordClient discordClient = null;
            try
            {
                discordClient = new DiscordClient(new DiscordConfiguration
                {
                    Token = config["freeman-token"],
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

        private static async Task OnMessageCreated(DiscordClient dc, MessageCreateEventArgs e)
        {
            if (e.Author.IsBot)
                return;

            var commands = _registry.ResolveAll<ICommand>();
            var config = _registry.Resolve<Configuration.IConfigurationProvider>().Configuration;

            var messageParts = e.Message.Content.Split(new[] { " ", "\t", "\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var commandStartIndex = messageParts.Select((part, ndx) => WordIsCommandStart(part, config["freedman-command-prefix"]) ? ndx : -1)
                .Where(x => x != -1)
                .FirstOrDefault();

            if (commandStartIndex + 1 >= messageParts.Length)
                return;

            messageParts = messageParts.Skip(commandStartIndex + 1).ToArray();

            foreach (var command in commands.Where(c => c.IsHandlerFor(messageParts[0])))
            {
                await command.HandleCommandAsync(messageParts, e.Message, e.Guild);
            }
        }

        private static bool WordIsCommandStart(string word, string commandPrefix)
        {
            return string.Equals(commandPrefix, word, StringComparison.OrdinalIgnoreCase);
        }
    }
}
