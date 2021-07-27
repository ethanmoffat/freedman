namespace freedman.Converters.Length
{
    public class NanometerConverter : LengthConverter
    {
        protected override double Factor => 1000000000.0;

        protected override string UnitRepresentation => "nm";

        public NanometerConverter()
            : base("^(nm|nanomet(re|er))s?$")
        {
        }
    }
}
