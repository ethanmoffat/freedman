namespace freedman.Converters.Length
{
    public class MillimeterConverter : LengthConverter
    {
        protected override double Factor => 0.001;

        protected override string UnitRepresentation => "mm";

        public MillimeterConverter()
            : base("^(mm|millimet(re|er))s?$")
        {
        }
    }
}
