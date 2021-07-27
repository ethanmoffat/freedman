namespace freedman.Converters.Length
{
    public class BigMacConverter : LengthConverter
    {
        // 2.75in -> m
        protected override double Factor => 2.75 / 39.3700787;

        protected override string UnitRepresentation => "big macs";

        public BigMacConverter()
            : base("^big macs?$")
        {
        }
    }
}
