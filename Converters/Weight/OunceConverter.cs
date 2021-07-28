using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Weight
{
    [AutoMappedType]
    public class OunceConverter : WeightConverter
    {
        public override IUnit DefaultTarget => new Unit.Volume(0.0, "g");

        protected override string UnitRepresentation => "oz";

        protected override double Factor => .45359237 / 16;

        public OunceConverter()
            : base("^(oz|ounce)s?$")
        {
        }
    }
}
