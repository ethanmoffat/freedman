using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class LightYearConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "au");

        protected override double Factor => 9460730472580800.0;

        protected override string UnitRepresentation => "ly";

        public LightYearConverter()
            : base("^(ly|(light( |-)?year)s?)$")
        {
        }
    }
}
