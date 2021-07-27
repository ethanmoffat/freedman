using AutomaticTypeMapper;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class MileConverter : LengthConverter
    {
        protected override double Factor => 1609.344;

        protected override string UnitRepresentation => "mi";

        public MileConverter()
            : base("^mi(le)?s?$")
        {
        }
    }
}
