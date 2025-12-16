using System.Runtime.CompilerServices;

namespace YoteiLib.Core;

public class Yotei
{
    public TaskRepository Tasks {get; set;}
    public ResourceRepository Resources { get; set; } 
    public ActorRepository Actors { get; set; }
    public RoleRepository Roles { get; set; }
    
    public Yotei()
    {
        Roles = new RoleRepository();
        Actors = new ActorRepository(); 
        Resources = new ResourceRepository();
        Tasks = new TaskRepository(Actors);
    }
}