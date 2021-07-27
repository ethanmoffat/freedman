using freedman.Unit;

namespace freedman.Converters.Temperature
{
    public abstract class TemperatureConverter : BaseConverter<Unit.Temperature>
    {
        protected override string SIRepresentation => "k";

        protected TemperatureConverter(string pattern)
            : base(pattern)
        {
        }

        public override IUnit UnitFactory(double value, string units = null)
        {
            return new Unit.Temperature(value, units);
        }
    }
}
