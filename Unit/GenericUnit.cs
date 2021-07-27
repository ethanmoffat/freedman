namespace freedman.Unit
{
    public class GenericUnit : IUnit
    {
        public double Value { get; }

        public string Units { get; }

        public GenericUnit(double value, string units)
        {
            Value = value;
            Units = units;
        }

        public override string ToString()
        {
            return $"{Value} {Units}";
        }
    }
}
