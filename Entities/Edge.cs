namespace YoteiLib.Core;

public enum EdgeType
{
    Block,
    Unblock
}

public class TaskEdge: IEntity
{
    
    public Id Id { get; init; }
    public Id From { get; set; }
    public Id To { get; set; }
    public EdgeType Type { get; set; }

    public TaskEdge()
    {
        Id = new Id();
    }
    public bool CanComplete()
    {
        return Type != EdgeType.Block;
    }

    public override bool Equals(object? obj) =>
        obj is TaskEdge e && e.From == From && e.To == To;

    public override int GetHashCode() => HashCode.Combine(From, To);


}