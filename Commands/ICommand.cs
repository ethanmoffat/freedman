using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace freedman.Commands
{
    public interface ICommand
    {
        string Name { get; }

        bool IsHandlerFor(string command);

        Task HandleCommandAsync(string[] input, DiscordMessage message, DiscordGuild guild);
    }
}