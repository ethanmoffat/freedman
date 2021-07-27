using freedman.Unit;

namespace freedman.Parser
{
    public interface IUnitParser
    {
        (IUnit Value, IUnit Target) Parse(string message);
    }
}
