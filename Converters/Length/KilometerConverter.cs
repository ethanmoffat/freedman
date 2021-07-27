namespace freedman.Converters.Length
{
    public class KilometerConverter : LengthConverter
    {
        // Meters * .001 = Kilometers
        protected override double Factor => 0.001;

        protected override string UnitRepresentation => "km";

        public KilometerConverter()
            : base("^(km|kilomet(re|er))s?$")
        {
        }
    }
}
