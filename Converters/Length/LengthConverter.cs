using freedman.Unit;
using System;
using System.Text.RegularExpressions;

namespace freedman.Converters.Length
{
    public abstract class LengthConverter : IUnitConverter
    {
        private readonly Regex _matcher;

        protected abstract double Factor { get; }

        protected abstract string UnitRepresentation { get; }

        protected LengthConverter(string pattern)
        {
            _matcher = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        }

        public virtual bool IsConverterFor(IUnit unit)
        {
            return _matcher.IsMatch(unit.Units);
        }

        /// <summary>
        /// Convert from meters to the specified unit
        /// </summary>
        public virtual IUnit FromSIUnit(IUnit source)
        {
            var converted = source switch
            {
                Unit.Length length => length.Value / Factor,
                _ => throw new ArgumentException($"source {source} is not a Length")
            };

            return new Unit.Length(converted, UnitRepresentation);
        }

        /// <summary>
        /// Convert from the specified unit to meters
        /// </summary>
        public virtual IUnit ToSIUnit(IUnit source)
        {
            var converted = source switch
            {
                Unit.Length length => length.Value * Factor,
                _ => throw new ArgumentException($"source {source} is not a Length")
            };

            return new Unit.Length(converted, "m");
        }
    }
}
