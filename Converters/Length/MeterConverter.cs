namespace freedman.Converters.Length
{
    public class MeterConverter : LengthConverter
    {
        protected override double Factor => 1.0;

        protected override string UnitRepresentation => "m";

        public MeterConverter()
            : base("^(m|met(er|re))s?$")
        {
        }
    }
}
