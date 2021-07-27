namespace freedman.Converters.Length
{
    public class MicrometerConverter : LengthConverter
    {
        protected override double Factor => 1000000.0;

        protected override string UnitRepresentation => "μm";

        public MicrometerConverter()
            : base("^(μm|micro(met(re|er)|n))s?$")
        {
        }
    }
}
