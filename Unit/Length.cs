namespace freedman.Unit
{
    public class Length : IUnit
    {
        public double Value { get; }

        public string Units { get; }

        public Length(double value, string units)
        {
            Value = value;
            Units = units;
        }

        public static Length operator*(Length input, double scalar)
        {
            return new Length(input.Value * scalar, input.Units);
        }

        public static Length operator/(Length input, double scalar)
        {
            return new Length(input.Value / scalar, input.Units);
        }
    }
}
