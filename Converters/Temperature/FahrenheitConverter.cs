using AutomaticTypeMapper;

namespace freedman.Converters.Temperature
{
    [AutoMappedType]
    public class FahrenheitConverter : TemperatureConverter
    {
        protected override double Factor => 1.0 / 1.8;

        protected override double TotalOffset => 273.15;

        protected override double ValueOffset => 32.0;

        protected override string UnitRepresentation => "f";

        public FahrenheitConverter()
            : base("(degrees)?(f|farenheit)")
        {
        }
    }
}
