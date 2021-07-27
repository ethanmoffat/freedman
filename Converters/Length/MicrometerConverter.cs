using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class MicrometerConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "nm");

        protected override double Factor => 0.000001;

        protected override string UnitRepresentation => "μm";

        public MicrometerConverter()
            : base("^(μm|micro(met(re|er)|n))s?$")
        {
        }
    }
}
