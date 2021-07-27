﻿using AutomaticTypeMapper;
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

        public (IUnit Value, IUnit Target) Parse(string message)
        {
            var messageParts = message.Split(" ");

            double quantity = 0;
            var units = string.Empty;
            if (!double.TryParse(messageParts[1], out quantity))
            {
                var filtered = new string(messageParts[1].Where(c => char.IsDigit(c) || c == '.').ToArray());
                if (!double.TryParse(filtered, out quantity))
                {
                    quantity = 1;
                }

                units = messageParts[1].Substring(messageParts[1].IndexOf(filtered) + filtered.Length);
                if (messageParts.Length > 2 && IsValidSecondWordUnit(units, messageParts[2]))
                {
                    units += " " + messageParts[2];
                }
            }
            else
            {
                if (messageParts.Length <= 2)
                {
                    throw new ArgumentException("Quantity and unit are required");
                }

                units = messageParts[2];
                if (messageParts.Length > 3 && IsValidSecondWordUnit(units, messageParts[3]))
                {
                    units += " " + messageParts[3];
                }
            }

            var value = _unitFactory.UnitFromUnits(quantity, units);

            var target = string.Empty;
            if (messageParts.Any(x => string.Equals(x, "to", StringComparison.OrdinalIgnoreCase)))
            {
                messageParts = messageParts.SkipWhile(x => !string.Equals(x, "to", StringComparison.OrdinalIgnoreCase)).Skip(1).ToArray();
                if (!messageParts.Any())
                    throw new ArgumentException("Target unit must be specified");

                target = messageParts[0];
                if (messageParts.Length > 1 && IsValidSecondWordUnit(target, messageParts[1]))
                    target += $" {messageParts[1]}";
            }

            return (value, _unitFactory.UnitFromUnits(0, target));
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
