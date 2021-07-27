namespace freedman.Converters.Length
{
    public class InchConverter : LengthConverter
    {
        protected override double Factor => 1.0 / 39.3700787;

        protected override string UnitRepresentation => "in";

        public InchConverter()
            : base("^in(ch(es)?)?$")
        {
        }
    }
}
