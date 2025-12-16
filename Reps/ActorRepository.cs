using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace YoteiLib.Core;

public class ActorRepository: IRepository<Actor>, IActorsMap
{
    private readonly Dictionary<Id, Actor> _actors = new();
    private Actor _current;
    
    public Actor GetCurrent { get =>  _current; }

    public ActorRepository()
    {
        SetCurrent(Create()); // всегда должен быть хотя бы 1
    }
    public IEnumerator<Actor> GetEnumerator()
    {
        return _actors.Values.GetEnumerator();
    }
    
    public Result SetCurrent(Actor actor)
    {
        if(!_actors.TryGetValue(actor.Id, out _current))
            return Result.ActorNotFound;
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Actor Create()
    {
        Actor defaultActor = new Actor();
        Add(defaultActor);
        return defaultActor;
    }

    public Actor? GetById(Id id)
    {
        return _actors.TryGetValue(id, out var actor) ? actor : null;
    }
    
    public Result TryFind(Func<Actor, bool> predicate, out Actor entity)
    {
        entity = _actors.Values.FirstOrDefault(predicate);
        if (entity == null)
        {
            return Result.ActorNotFound;
        }

        return Result.OK;
    }
    
    public IReadOnlyList<Actor> GetAll()
    {
        return _actors.Values.ToList();
    }

    public Result Add(Actor actor)
    {
        if(_actors.TryAdd(actor.Id, actor)) return Result.OK;
        return Result.ActorAlreadyExist;
        
    }

    public Result Delete(Actor entity)
    {
        if (!_actors.Remove(entity.Id))
        {
            return Result.ActorNotFound;
        }

        // Если удалили текущего, переключаемся на любой оставшийся или создаем нового
        if (Equals(_current, entity))
        {
            if (_actors.Count > 0)
            {
                _current = _actors.Values.First();
            }
            else
            {
                _current = Create();
            }
        }

        return Result.OK;
    }
    
    public Result Delete(Func<Actor, bool> predicate)
    {
        var actor = _actors.Values.FirstOrDefault(predicate);
        if (actor == null)
        {
            return Result.ActorNotFound;
        }

        return Delete(actor);
    }

 
}