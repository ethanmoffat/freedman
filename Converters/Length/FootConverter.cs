using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class FootConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "cm");

        protected override double Factor => 1 / 3.2808398950131;

        protected override string UnitRepresentation => "ft";

        public FootConverter()
            : base("^(ft|foot|feet)$")
        {
        }
    }
}
