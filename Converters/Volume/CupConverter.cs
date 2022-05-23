using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Volume
{
    [AutoMappedType]
    public class CupConverter : VolumeConverter
    {
        public override IUnit DefaultTarget => new Unit.Volume(0.0, "ml");

        protected override string UnitRepresentation => "cup";

        protected override double Factor => 8.0 / 33.814023;

        public CupConverter()
            : base("^cups?$")
        {
        }
    }
}
