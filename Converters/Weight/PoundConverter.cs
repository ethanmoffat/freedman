using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Weight
{
    [AutoMappedType]
    public class PoundConverter : WeightConverter
    {
        public override IUnit DefaultTarget => new Unit.Volume(0.0, "kg");

        protected override string UnitRepresentation => "lbs";

        protected override double Factor => .45359237;

        public PoundConverter()
            : base("^(lb|pound|\\#)s?$")
        {
        }
    }
}
