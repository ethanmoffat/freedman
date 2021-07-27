using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class MileConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "km");

        protected override double Factor => 1609.344;

        protected override string UnitRepresentation => "mi";

        public MileConverter()
            : base("^mi(le)?s?$")
        {
        }
    }
}
