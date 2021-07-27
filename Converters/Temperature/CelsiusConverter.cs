using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Temperature
{
    [AutoMappedType]
    public class CelsiusConverter : TemperatureConverter
    {
        public override IUnit DefaultTarget => new Unit.Temperature(0, "f");

        protected override double Factor => 1;

        protected override double TotalOffset => 273.15;

        protected override string UnitRepresentation => "c";

        public CelsiusConverter()
            : base("^(degrees )?(c|celsius)$")
        {
        }
    }
}
