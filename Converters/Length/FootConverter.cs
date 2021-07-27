using AutomaticTypeMapper;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class FootConverter : LengthConverter
    {
        protected override double Factor => 1 / 3.2808398950131;

        protected override string UnitRepresentation => "ft";

        public FootConverter()
            : base("^(ft|foot|feet)$")
        {
        }
    }
}
