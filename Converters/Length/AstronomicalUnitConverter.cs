using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class AstronomicalUnitConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "ly");

        protected override double Factor => 149597870700.0;

        protected override string UnitRepresentation => "au";

        public AstronomicalUnitConverter()
            : base("^(au|astronomical units?)$")
        {
        }
    }
}
