namespace YoteiLib.Core;

public interface IRepository<E> : IEnumerable<E> where E:IEntity
{
    public E Create();
    public E? GetById(Id id);
    public Result TryFind(Func<E, bool> predicate, out E entity);
    public IReadOnlyList<E> GetAll();
    public Result Add(E entity); 
    public Result Delete(E entity); 
    public Result Delete(Func<E, bool> predicate); 
    
    
}