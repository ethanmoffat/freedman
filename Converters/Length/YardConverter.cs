namespace freedman.Converters.Length
{
    public class YardConverter : LengthConverter
    {
        protected override double Factor => 0.9144;

        protected override string UnitRepresentation => "yd";

        public YardConverter()
            : base("^(yd|yards?)$")
        {
        }
    }
}
