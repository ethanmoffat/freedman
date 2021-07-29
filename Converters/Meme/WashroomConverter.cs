using AutomaticTypeMapper;
using freedman.Unit;
using System;
using System.Threading.Tasks;

namespace freedman.Converters.Meme
{
    [AutoMappedType]
    public class WashroomConverter : IUnitConverter
    {
        public IUnit DefaultTarget => new GenericUnit(0, "bathroom");

        public bool IsConverterFor(IUnit unit)
        {
            return string.Equals(unit.Units, "washroom", StringComparison.OrdinalIgnoreCase);
        }

        public Task<IUnit> FromSIUnitAsync(IUnit source)
        {
            return Task.FromResult(UnitFactory(source.Value, "washroom"));
        }

        public Task<IUnit> ToSIUnitAsync(IUnit source)
        {
            return Task.FromResult(UnitFactory(source.Value, "bathroom"));
        }

        public IUnit UnitFactory(double value, string units)
        {
            return new GenericUnit(value, units);
        }
    }
}
