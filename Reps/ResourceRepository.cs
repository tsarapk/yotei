using System.Collections;

namespace YoteiLib.Core;

public class ResourceRepository: IRepository<Resource>, IResourcesMap
{
    private List<Resource> _resources = new List<Resource>();
    public IEnumerator<Resource> GetEnumerator()
    {
        return _resources.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Resource Create()
    {
        Resource resource = new Resource("Empty", 0);
        _resources.Add(resource);
        return resource;
    }

    public Resource Create(string resourceName, double startValue)
    {
        Resource resource = new Resource(resourceName, startValue);
        _resources.Add(resource);
        return resource;
    }
    

    public Resource? GetById(Id id)
    {
        return _resources.FirstOrDefault(t => t.Id == id); 
    }

    public Resource Find(Func<Resource, bool> predicate)
    {
        Resource? resource = _resources.FirstOrDefault(predicate);
        if (resource == null) throw new Exception("Resource not found");
        return resource;
    }

    public Result TryFind(Func<Resource, bool> predicate, out Resource entity)
    {
        entity = _resources.FirstOrDefault(predicate);
        if (entity == null) return Result.ResourceNotFound;
        return Result.OK;
    }

    public IReadOnlyList<Resource> GetAll()
    {
        return _resources;
    }
    
    public Result Delete(Resource entity)
    {
        if(_resources.Remove(entity)) return true;
        return Result.ResourceNotFound;
    }
    public Result Delete(Func<Resource, bool> predicate)
    {
        if(_resources.Remove(Find(predicate))) return true; //TryFind mb?
        return Result.ResourceNotFound;
    }

    public Result Add(Resource entity)
    {
        
        if(entity == null) throw new ArgumentNullException(nameof(entity));
        if(_resources.Contains(entity)) return Result.ResourceNotFound;
        _resources.Add(entity);
        return true;
    }

    public override string ToString()
    {
        string result = "";
        _resources.ForEach(s => result += s.Name);
        return result;
    }
}