namespace freedman.Converters.Length
{
    public class AstronomicalUnitConverter : LengthConverter
    {
        protected override double Factor => 149597870700.0;

        protected override string UnitRepresentation => "au";

        public AstronomicalUnitConverter()
            : base("^(au|astronomical units?)$")
        {
        }
    }
}
