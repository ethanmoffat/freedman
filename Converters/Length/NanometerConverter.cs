using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class NanometerConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "micron");

        protected override double Factor => 0.000000001;

        protected override string UnitRepresentation => "nm";

        public NanometerConverter()
            : base("^(nm|nanomet(re|er))s?$")
        {
        }
    }
}
