using freedman.Unit;

namespace freedman.Converters.Length
{
    public abstract class LengthConverter : BaseConverter<Unit.Length>
    {
        protected override string SIRepresentation => "m";

        protected LengthConverter(string pattern)
            : base(pattern)
        {
        }

        public override IUnit UnitFactory(double value, string units)
        {
            return new Unit.Length(value, units);
        }
    }
}
