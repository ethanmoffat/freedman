using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Weight
{
    [AutoMappedType]
    public class GramConverter : WeightConverter
    {
        public override IUnit DefaultTarget => new Unit.Volume(0.0, "oz");

        protected override string UnitRepresentation => "g";

        protected override double Factor => .001;

        public GramConverter()
            : base("^(g|gram)s?$")
        {
        }
    }
}
