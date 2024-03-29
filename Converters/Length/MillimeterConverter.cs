﻿using AutomaticTypeMapper;
using freedman.Unit;

namespace freedman.Converters.Length
{
    [AutoMappedType]
    public class MillimeterConverter : LengthConverter
    {
        public override IUnit DefaultTarget => new Unit.Length(0, "in");

        protected override double Factor => 0.001;

        protected override string UnitRepresentation => "mm";

        public MillimeterConverter()
            : base("^(mm|millimet(re|er))s?$")
        {
        }
    }
}
