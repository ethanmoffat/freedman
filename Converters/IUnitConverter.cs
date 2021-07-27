using freedman.Unit;

namespace freedman.Converters
{
    public interface IUnitConverter
    {
        bool IsConverterFor(IUnit unit);

        IUnit ToSIUnit(IUnit source);

        IUnit FromSIUnit(IUnit source);
    }
}
