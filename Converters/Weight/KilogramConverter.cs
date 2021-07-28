using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Weight
{
    [AutoMappedType]
    public class KilogramConverter : WeightConverter
    {
        public override IUnit DefaultTarget => new Unit.Volume(0.0, "lbs");

        protected override string UnitRepresentation => "kg";

        protected override double Factor => 1;

        public KilogramConverter()
            : base("^(kg|kilogram)s?$")
        {
        }
    }
}
