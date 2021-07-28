using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Volume
{
    [AutoMappedType]
    public class FluidOunceConverter : VolumeConverter
    {
        public override IUnit DefaultTarget => new Unit.Volume(0.0, "ml");

        protected override string UnitRepresentation => "fl oz";

        protected override double Factor => 1.0 / 33.814023;

        public FluidOunceConverter()
            : base("^((fl|fluid) (oz|ounce)s?)$")
        {
        }
    }
}
