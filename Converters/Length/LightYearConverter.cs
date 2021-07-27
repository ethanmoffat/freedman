using AutomaticTypeMapper;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class LightYearConverter : LengthConverter
    {
        protected override double Factor => 9460730472580800.0;

        protected override string UnitRepresentation => "ly";

        public LightYearConverter()
            : base("^(ly|(light( |-)?year)s?)$")
        {
        }
    }
}
