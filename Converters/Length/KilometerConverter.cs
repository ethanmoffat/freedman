using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class KilometerConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "mi");

        // Meters * .001 = Kilometers
        protected override double Factor => 1000.0;

        protected override string UnitRepresentation => "km";

        public KilometerConverter()
            : base("^(km|kilomet(re|er))s?$")
        {
        }
    }
}
