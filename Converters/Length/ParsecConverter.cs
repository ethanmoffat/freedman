using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class ParsecConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "ly");

        // https://en.wikipedia.org/wiki/Parsec#Calculating_the_value_of_a_parsec
        protected override double Factor => 30856775814913673.0;

        protected override string UnitRepresentation => "pc";

        public ParsecConverter()
            : base("^(pc|parsecs?)$")
        {
        }
    }
}
