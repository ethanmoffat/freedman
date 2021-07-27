using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class FootballFieldConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "yds");

        // 360ft -> m
        protected override double Factor => 360 / 3.2808398950131;

        protected override string UnitRepresentation => "football fields";

        public FootballFieldConverter()
            : base("^football fields?$")
        {
        }
    }
}
