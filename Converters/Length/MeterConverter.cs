using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class MeterConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "ft");

        protected override double Factor => 1.0;

        protected override string UnitRepresentation => "m";

        public MeterConverter()
            : base("^(m|met(er|re))s?$")
        {
        }
    }
}
