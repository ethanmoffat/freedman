using AutomaticTypeMapper;
using freedman.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace freedman.Unit
{
    [AutoMappedType]
    public class UnitFactory : IUnitFactory
    {
        private readonly IEnumerable<IUnitConverter> _converters;

        public UnitFactory(IEnumerable<IUnitConverter> converters)
        {
            _converters = converters;
        }

        public IUnit UnitFromUnits(double value, string units)
        {
            var dummyUnit = new GenericUnit(0, units);
            var converter = _converters.SingleOrDefault(x => x.IsConverterFor(dummyUnit));
            return converter?.UnitFactory(value, units) ?? throw new ArgumentException($"Unit {units} is not supported", nameof(units));
        }
    }
}
