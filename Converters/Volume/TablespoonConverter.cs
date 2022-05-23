using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Volume
{
    [AutoMappedType]
    public class TablespoonConverter : VolumeConverter
    {
        public override IUnit DefaultTarget => new Unit.Volume(0.0, "ml");

        protected override string UnitRepresentation => "tbsp";

        protected override double Factor => 0.5 / 33.814023;

        public TablespoonConverter()
            : base("^((tbsp|tablespoon)s?)$")
        {
        }
    }
}
