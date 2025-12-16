namespace YoteiLib.Core;

public class Actor: IEntity
{
    public Id Id { get; init; }
    public string Name { get; private set; } 
    public Role Role;
    public string? Username { get; private set; }
    public string? PasswordHash { get; private set; }
    
    public Actor()
    {
        Id = new Id();
        Name = Id.ToString();
        Role = new Role();
    }

    public Actor SetName(string name)
    {
        Name = name;
        return this;
    }
  
    public Actor SetCredentials(string username, string passwordHash)
    {
        Username = username;
        PasswordHash = passwordHash;
        return this;
    }

    public override string ToString()
    {
        return Name;
    }
}