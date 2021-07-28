using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Volume
{
    [AutoMappedType]
    public class LiterConverter : VolumeConverter
    {
        public override IUnit DefaultTarget => new Unit.Volume(0.0, "gal");

        protected override string UnitRepresentation => "l";

        protected override double Factor => 1;

        public LiterConverter()
            : base("^l(it(er|re)s?)?$")
        {
        }
    }
}
