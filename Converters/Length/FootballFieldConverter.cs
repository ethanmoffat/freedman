namespace freedman.Converters.Length
{
    public class FootballFieldConverter : LengthConverter
    {
        // 360ft -> m
        protected override double Factor => 360 / 3.2808399;

        protected override string UnitRepresentation => "football fields";

        public FootballFieldConverter()
            : base("^football fields?$")
        {
        }
    }
}
