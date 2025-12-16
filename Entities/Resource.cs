namespace YoteiLib.Core;

public class Resource : IEntity
{
    public Id Id { get; init; }
    public string Name { get; private set; }
    public double Value { get; private set; }

    public Resource(string name, double startValue)
    {
        Id = new();
        Name = name;
        Value = startValue;
    }

    public Resource SetName(string name)
    {
        Name =  name;
        return this;
    }

    public Resource SetValue(double value)
    {
        Value = value;
        return this;
    }
}