namespace YoteiLib.Core;

public class Id
{
    public Guid guid;
    
    public Id()
    {
        guid = Guid.NewGuid();
    }
    
    public static bool operator ==(Id a, Id b)
    {
        return a.guid == b.guid;
    }

    public static bool operator !=(Id a, Id b)
    {
        return !(a == b);
    }

    public override bool Equals(object? o)
    {
        if(GetType() != o.GetType()) return false;
        if(o is Id other) return guid.Equals(other.guid);
        return false;
    }

    public override string ToString()
    {
        return guid.ToString();
    }

    public override int GetHashCode()
    {
        return guid.GetHashCode();
    }
}