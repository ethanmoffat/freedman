using freedman.Unit;

namespace freedman.Converters.Weight
{
    public abstract class WeightConverter : BaseConverter<Unit.Weight>
    {
        protected override string SIRepresentation => "kg";

        protected WeightConverter(string pattern)
            : base(pattern)
        {
        }

        public override IUnit UnitFactory(double value, string units = null)
        {
            return new Unit.Weight(value, units);
        }
    }
}
