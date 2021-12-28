using AutomaticTypeMapper;
using freedman.Unit;
using System;
using System.Linq;

namespace freedman.Parser
{
    [AutoMappedType]
    public class UnitParser : IUnitParser
    {
        private readonly IUnitFactory _unitFactory;

        public UnitParser(IUnitFactory unitFactory)
        {
            _unitFactory = unitFactory;
        }

        public (IUnit Value, IUnit Target) Parse(string[] splitMessage)
        {
            var quantity = 0.0;
            var units = string.Empty;
            if (!double.TryParse(splitMessage[1], out quantity))
            {
                var filtered = new string(splitMessage[1].Where(c => char.IsDigit(c) || c == '.').ToArray());
                if (splitMessage[1].StartsWith('-'))
                {
                    filtered = $"-{filtered}";
                }

                if (!double.TryParse(filtered, out quantity))
                {
                    quantity = 1;
                }

                units = splitMessage[1].Substring(splitMessage[1].IndexOf(filtered) + filtered.Length);
                if (splitMessage.Length > 2 && IsValidSecondWordUnit(units, splitMessage[2]))
                {
                    units += " " + splitMessage[2];
                }
            }
            else if (splitMessage.Length <= 2)
            {
                throw new ArgumentException("Quantity and unit are required");
            }
            else
            {
                units = splitMessage[2];
                if (splitMessage.Length > 3 && IsValidSecondWordUnit(units, splitMessage[3]))
                {
                    units += " " + splitMessage[3];
                }
            }

            var value = _unitFactory.UnitFromUnits(quantity, units);

            var target = string.Empty;
            if (splitMessage.Any(x => string.Equals(x, "to", StringComparison.OrdinalIgnoreCase)))
            {
                splitMessage = splitMessage.SkipWhile(x => !string.Equals(x, "to", StringComparison.OrdinalIgnoreCase)).Skip(1).ToArray();
                if (!splitMessage.Any())
                    throw new ArgumentException("Target unit must be specified");

                target = splitMessage[0];
                if (splitMessage.Length > 1 && IsValidSecondWordUnit(target, splitMessage[1]))
                    target += $" {splitMessage[1]}";

                return (value, _unitFactory.UnitFromUnits(0, target));
            }
            else
            {
                return (value, _unitFactory.DefaultTargetUnit(value));
            }
        }

        private static bool IsValidSecondWordUnit(string firstPart, string secondPart)
        {
            var secondPartLower = secondPart.ToLower();
            switch (firstPart.ToLower())
            {
                case "degrees":
                    return secondPartLower == "f" || secondPartLower == "c" || secondPartLower == "k"
                        || secondPartLower == "farenheit" || secondPartLower == "celsius" || secondPartLower == "kelvin";
                case "fl":
                case "fluid":
                    return secondPartLower == "oz" || secondPartLower == "ozs" || secondPartLower == "ounce" || secondPartLower == "ounces";
                case "light":
                    return secondPartLower == "years";
                case "astronomical":
                    return secondPartLower == "units";
                case "big":
                    return secondPartLower == "mac" || secondPartLower == "macs";
                case "football":
                    return secondPartLower == "field" || secondPartLower == "fields";
            }

            return false;
        }
    }
}
