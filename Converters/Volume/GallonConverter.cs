using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Volume
{
    [AutoMappedType]
    public class GallonConverter : VolumeConverter
    {
        public override IUnit DefaultTarget => new Unit.Volume(0.0, "l");

        protected override string UnitRepresentation => "gal";

        protected override double Factor => 3.785411784;

        public GallonConverter()
            : base("^gal(lons?)?$")
        {
        }
    }
}
