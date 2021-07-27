using freedman.Unit;
using System;
using System.Text.RegularExpressions;

namespace freedman.Converters
{
    public abstract class BaseConverter<T> : IUnitConverter
        where T : IUnit
    {
        private readonly Regex _matcher;

        public abstract IUnit DefaultTarget { get; }

        protected abstract double Factor { get; }

        protected virtual double TotalOffset { get; } = 0;

        protected virtual double ValueOffset { get; } = 0;

        protected abstract string UnitRepresentation { get; }

        protected abstract string SIRepresentation { get; }

        protected BaseConverter(string pattern)
        {
            _matcher = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        }

        public virtual bool IsConverterFor(IUnit unit)
        {
            return _matcher.IsMatch(unit.Units);
        }

        public virtual IUnit FromSIUnit(IUnit source)
        {
            var converted = source switch
            {
                T matchedType => (matchedType.Value - TotalOffset) / Factor + ValueOffset,
                _ => throw new ArgumentException($"source {source} is not a {typeof(T).Name}")
            };

            return UnitFactory(converted, UnitRepresentation);
        }

        public virtual IUnit ToSIUnit(IUnit source)
        {
            var converted = source switch
            {
                T matchedType => (matchedType.Value - ValueOffset) * Factor + TotalOffset,
                _ => throw new ArgumentException($"source {source} is not a {typeof(T).Name}")
            };

            return UnitFactory(converted, SIRepresentation);
        }

        public abstract IUnit UnitFactory(double value, string units);
    }
}
