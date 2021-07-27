using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class YardConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "m");

        protected override double Factor => 0.9144;

        protected override string UnitRepresentation => "yd";

        public YardConverter()
            : base("^(yd|yards?)$")
        {
        }
    }
}
