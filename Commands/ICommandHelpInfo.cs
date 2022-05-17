namespace freedman.Commands
{
    public interface ICommandHelpInfo
    {
        string Name { get; }

        string GetUsage();

        string GetDescription();
    }
}
