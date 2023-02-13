namespace SodaMachineLibrary.Models;

public class SodaModel
{
    public SodaModel(string name, string slot)
    {
        Name = name;
        SlotOccupied = slot;
    }
    public string Name { get; }
    public string SlotOccupied { get; }

    public override string ToString()
    {
        return base.ToString();
    }
}
