using AutomaticTypeMapper;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class NanometerConverter : LengthConverter
    {
        protected override double Factor => 0.000000001;

        protected override string UnitRepresentation => "nm";

        public NanometerConverter()
            : base("^(nm|nanomet(re|er))s?$")
        {
        }
    }
}
