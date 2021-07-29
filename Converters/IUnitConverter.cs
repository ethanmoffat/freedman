using freedman.Unit;
using System.Threading.Tasks;

namespace freedman.Converters
{
    public interface IUnitConverter
    {
        IUnit DefaultTarget { get; }

        bool IsConverterFor(IUnit unit);

        Task<IUnit> ToSIUnitAsync(IUnit source);

        Task<IUnit> FromSIUnitAsync(IUnit source);

        IUnit UnitFactory(double value, string units);
    }
}
