using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class CentimeterConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "in");

        protected override double Factor => 0.01;

        protected override string UnitRepresentation => "cm";

        public CentimeterConverter()
            : base("^(cm|centimet(re|er))s?$")
        {
        }
    }
}
