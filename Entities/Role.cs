namespace YoteiLib.Core;

public enum RolePriv
{
    CanReadProject,
    CanWriteProject,
    FullAccess,
    CanAddNewTasks,
    CanCompleteTasks
}

public class Role : IEntity
{
    public Id Id { get; init; }

    
    public string Name { get; set; } = string.Empty;

    
    public int Strength { get; set; } = 0;


    public List<RolePriv> privs = new();

    public Role()
    {
        Id = new Id();
    }
}