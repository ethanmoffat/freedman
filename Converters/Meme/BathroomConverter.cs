using AutomaticTypeMapper;
using freedman.Unit;
using System;
using System.Threading.Tasks;

namespace freedman.Converters.Meme
{
    [AutoMappedType]
    public class BathroomConverter : IUnitConverter
    {
        public IUnit DefaultTarget => new GenericUnit(0, "washroom");

        public bool IsConverterFor(IUnit unit)
        {
            return string.Equals(unit.Units, "bathroom", StringComparison.OrdinalIgnoreCase);
        }

        public Task<IUnit> FromSIUnitAsync(IUnit source)
        {
            return Task.FromResult(source);
        }

        public Task<IUnit> ToSIUnitAsync(IUnit source)
        {
            return Task.FromResult(source);
        }

        public IUnit UnitFactory(double value, string units)
        {
            return new GenericUnit(value, units);
        }
    }
}
