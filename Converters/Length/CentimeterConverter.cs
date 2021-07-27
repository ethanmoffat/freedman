namespace freedman.Converters.Length
{
    public class CentimeterConverter : LengthConverter
    {
        protected override double Factor => 0.01;

        protected override string UnitRepresentation => "cm";

        public CentimeterConverter()
            : base("^(cm|centimet(re|er))s?$")
        {
        }
    }
}
