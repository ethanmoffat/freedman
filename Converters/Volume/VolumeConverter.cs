using freedman.Unit;

namespace freedman.Converters.Volume
{
    public abstract class VolumeConverter : BaseConverter<Unit.Volume>
    {
        protected override string SIRepresentation => "l";

        protected VolumeConverter(string pattern)
            : base(pattern)
        {
        }

        public override IUnit UnitFactory(double value, string units = null)
        {
            return new Unit.Volume(value, units);
        }
    }
}
