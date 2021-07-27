using AutomaticTypeMapper;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class MicrometerConverter : LengthConverter
    {
        protected override double Factor => 0.000001;

        protected override string UnitRepresentation => "μm";

        public MicrometerConverter()
            : base("^(μm|micro(met(re|er)|n))s?$")
        {
        }
    }
}
