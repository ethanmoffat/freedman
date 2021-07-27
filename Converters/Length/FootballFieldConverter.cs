using AutomaticTypeMapper;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class FootballFieldConverter : LengthConverter
    {
        // 360ft -> m
        protected override double Factor => 360 / 3.2808398950131;

        protected override string UnitRepresentation => "football fields";

        public FootballFieldConverter()
            : base("^football fields?$")
        {
        }
    }
}
