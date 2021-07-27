using AutomaticTypeMapper;

namespace freedman.Converters.Temperature
{
    [AutoMappedType]
    public class KelvinConverter : TemperatureConverter
    {
        protected override double Factor => 1;

        protected override string UnitRepresentation => "k";

        public KelvinConverter()
            : base("^(degrees )?(k|kelvin)$")
        {
        }
    }
}
