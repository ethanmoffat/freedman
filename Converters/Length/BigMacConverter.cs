using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class BigMacConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "football fields");

        // 2.75in -> m
        protected override double Factor => 2.75 / 39.3700787;

        protected override string UnitRepresentation => "big macs";

        public BigMacConverter()
            : base("^big macs?$")
        {
        }
    }
}
