using freedman.Unit;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public virtual Task<IUnit> FromSIUnitAsync(IUnit source)
        {
            var converted = source switch
            {
                T matchedType => (matchedType.Value - TotalOffset) / Factor + ValueOffset,
                _ => throw new ArgumentException($"source {source} is not a {typeof(T).Name}")
            };

            return Task.FromResult(UnitFactory(converted, UnitRepresentation));
        }

        public virtual Task<IUnit> ToSIUnitAsync(IUnit source)
        {
            var converted = source switch
            {
                T matchedType => (matchedType.Value - ValueOffset) * Factor + TotalOffset,
                _ => throw new ArgumentException($"source {source} is not a {typeof(T).Name}")
            };

            return Task.FromResult(UnitFactory(converted, SIRepresentation));
        }

        public abstract IUnit UnitFactory(double value, string units);
    }
}
