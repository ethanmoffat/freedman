using AutomaticTypeMapper;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class InchConverter : LengthConverter
    {
        protected override double Factor => 0.0254;

        protected override string UnitRepresentation => "in";

        public InchConverter()
            : base("^in(ch(es)?)?$")
        {
        }
    }
}
