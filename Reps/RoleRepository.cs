using System.Collections;

namespace YoteiLib.Core;

public class RoleRepository : IRepository<Role>
{
    private readonly List<Role> _roles = new();

    public IEnumerator<Role> GetEnumerator()
    {
        return _roles.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Role Create()
    {
        var role = new Role
        {
            Name = "Новая роль"
        };
        _roles.Add(role);
        return role;
    }

    public Role? GetById(Id id)
    {
        return _roles.FirstOrDefault(r => r.Id.Equals(id));
    }

    public Result TryFind(Func<Role, bool> predicate, out Role entity)
    {
        entity = _roles.FirstOrDefault(predicate)!;
        if (entity == null)
        {
            return Result.RoleNotFound;
        }

        return Result.OK;
    }

    public IReadOnlyList<Role> GetAll()
    {
        return _roles;
    }

    public Result Add(Role entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (_roles.Contains(entity))
        {
            return Result.RoleAlreadyExist;
        }

        _roles.Add(entity);
        return Result.OK;
    }

    public Result Delete(Role entity)
    {
        if (_roles.Remove(entity))
        {
            return Result.OK;
        }

        return Result.RoleNotFound;
    }

    public Result Delete(Func<Role, bool> predicate)
    {
        var role = _roles.FirstOrDefault(predicate);
        if (role == null)
        {
            return Result.RoleNotFound;
        }

        return Delete(role);
    }
}