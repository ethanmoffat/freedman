using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Temperature
{
    [AutoMappedType]
    public class KelvinConverter : TemperatureConverter
    {
        public override IUnit DefaultTarget => new Unit.Temperature(0, "c");

        protected override double Factor => 1;

        protected override string UnitRepresentation => "k";

        public KelvinConverter()
            : base("^(degrees )?(k|kelvin)$")
        {
        }
    }
}
