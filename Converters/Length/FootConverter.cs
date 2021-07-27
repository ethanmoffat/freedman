namespace freedman.Converters.Length
{
    public class FootConverter : LengthConverter
    {
        protected override double Factor => 1.0 / 3.2808399;

        protected override string UnitRepresentation => "ft";

        public FootConverter()
            : base("^(ft|foot|feet)$")
        {
        }
    }
}
