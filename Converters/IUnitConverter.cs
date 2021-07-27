using freedman.Unit;

namespace freedman.Converters
{
    public interface IUnitConverter
    {
        IUnit DefaultTarget { get; }

        bool IsConverterFor(IUnit unit);

        IUnit ToSIUnit(IUnit source);

        IUnit FromSIUnit(IUnit source);

        IUnit UnitFactory(double value, string units);
    }
}
