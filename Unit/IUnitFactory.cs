namespace freedman.Unit
{
    public interface IUnitFactory
    {
        IUnit UnitFromUnits(double value, string units);
    }
}