using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Volume
{
    [AutoMappedType]
    public class MilliliterConverter : VolumeConverter
    {
        public override IUnit DefaultTarget => new Unit.Volume(0.0, "fl oz");

        protected override string UnitRepresentation => "ml";

        protected override double Factor => .001;

        public MilliliterConverter()
            : base("^(ml|millilit(er|re)s?)$")
        {
        }
    }
}
