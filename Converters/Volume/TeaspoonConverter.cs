using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Volume
{
    [AutoMappedType]
    public class TeaspoonConverter : VolumeConverter
    {
        public override IUnit DefaultTarget => new Unit.Volume(0.0, "ml");

        protected override string UnitRepresentation => "tsp";

        protected override double Factor => 0.16666666667 / 33.814023;

        public TeaspoonConverter()
            : base("^((tsp|teaspoon)s?)$")
        {
        }
    }
}
