using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class InchConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "cm");

        protected override double Factor => 0.0254;

        protected override string UnitRepresentation => "in";

        public InchConverter()
            : base("^in(ch(es)?)?$")
        {
        }
    }
}
