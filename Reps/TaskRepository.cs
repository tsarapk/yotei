using System.Collections;

namespace YoteiLib.Core;

public class TaskRepository : IRepository<TaskNode>
{
    private readonly Dictionary<Id, TaskNode> _tasks = new();
    private readonly Dictionary<Id, HashSet<TaskEdge>> _outgoing = new();
    private readonly Dictionary<Id, HashSet<TaskEdge>> _incoming = new();
    
    public event Action OnUpdate; 

    private IActorsMap _actors;

    public IReadOnlyList<TaskNode> GetIncoming(TaskNode task) 
        => GetRelations(task, _incoming, edge => edge.From);
    
    public IReadOnlyList<TaskNode> GetOutgoing(TaskNode task) 
        => GetRelations(task, _outgoing, edge => edge.To);
    
    public IReadOnlyList<TaskNode> GetIncomingRecursive(TaskNode task)
        => GetRecursiveRelations(task, _incoming, edge => edge.From);

    public IReadOnlyList<TaskNode> GetOutgoingRecursive(TaskNode task) 
        => GetRecursiveRelations(task, _outgoing, edge => edge.To);

    public TaskRepository(IActorsMap actors)
    {
        _actors = actors;
    }
    public TaskNode? GetById(Id id)
    {
        _tasks.TryGetValue(id, out var task);
        return task;
    }
    public Result TryFind(Func<TaskNode, bool> predicate, out TaskNode task)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));
        task = _tasks.Values.FirstOrDefault(predicate);
        return task != null;
    }
    
    public IReadOnlyList<TaskNode> GetAll()
    {
        return _tasks.Values.ToList();
    }

    public TaskNode Create()
    {
        TaskNode task = new TaskNode();
        Add(task);
        return task;
    }
    public TaskNode Create(string title)
    {
        TaskNode task = new TaskNode().SetTitle(title);
        Add(task);
        return task;
    }
    public Result Add(TaskNode task)
    {
        if (task == null)
            return Result.TaskNotFound;
        
        if(!_tasks.TryAdd(task.Id, task)) 
            return Result.TaskAlreadyExist;

        task.SetActor(_actors.GetCurrent);
        
        OnUpdate?.Invoke();
        return Result.OK;
    }

    public Result BindActor(TaskNode? task, Actor? actor)
    {
        if (task == null)
            return Result.TaskNotFound;
        if (actor == null)
            return Result.ActorNotFound;

        if(!_tasks.ContainsKey(task.Id))
           return Result.TaskNotFound;

        if (_actors.GetCurrent.Role.Strength >= actor.Role.Strength)
        {
            task.SetActor(actor);
            return true;
        }

        return false;
    }

    public void ForceComlete(TaskNode task)
    {
        TryComplete(task, out var uncompleted);
        foreach (var t in uncompleted)
        {
            GetById(t).SetStatus(TaskStatus.Completed);
        }
    }
    public Result TryComplete(TaskNode task, out IReadOnlyList<Id>? uncompleted)
    {
        uncompleted = null;
        
        if (task.Meta.PerfomedBy != _actors.GetCurrent)
            return Result.WrongActor; 
        if (!_incoming.TryGetValue(task.Id, out var incoming))
            return Result.OK;
        
        List<Id>? temp = null;
        foreach (var child in incoming)
        {
            temp = new List<Id>(incoming.Count);
            if (!child.CanComplete())
            {
                temp.Add(child.From);
            }
        }
        
        uncompleted = temp;
        if (temp != null) 
            return Result.ThereAreUncompletedTasks;
        
        task.SetStatus(TaskStatus.Completed);

        if (!_outgoing.TryGetValue(task.Id, out var outgoing))
            return Result.OK;
        
        foreach (var e in outgoing)
        {
                if(e.Type ==  EdgeType.Block)
                    e.Type = EdgeType.Unblock;
        }
        return Result.OK;
    }
    
    public TaskNode? Uncomplete(Id taskId)
    {
        if (_tasks.TryGetValue(taskId,  out var task))
        {
            if(task.IsCompleted) task.SetStatus(TaskStatus.Uncompleted);
            foreach (var e in  _incoming[task.Id])
            {
                e.Type = EdgeType.Block;
            }
        }
        return task;
    }

    public Result Delete(TaskNode task)
    {
        bool bridgeDependencies = true;
        
        if (task == null) throw new ArgumentNullException(nameof(task));
        if (!_tasks.ContainsKey(task.Id))
            return Result.TaskNotFound;

        var taskId = task.Id;

        var incoming = _incoming.TryGetValue(taskId, out var inEdges)
            ? inEdges.ToList()
            : new List<TaskEdge>();

        var outgoing = _outgoing.TryGetValue(taskId, out var outEdges)
            ? outEdges.ToList()
            : new List<TaskEdge>();

        if (bridgeDependencies)
        {
            foreach (var parentEdge in incoming)
            {
                foreach (var childEdge in outgoing)
                {
                    var parentId = parentEdge.From;
                    var childId = childEdge.To;

                    if (parentId == childId)
                        continue;

                    AddRelation(GetById(parentId), GetById(childId), EdgeType.Block);
                }
            }
        }

        foreach (var edge in incoming)
        {
            if (_outgoing.TryGetValue(edge.From, out var fromSet))
                fromSet.Remove(edge);
        }

        foreach (var edge in outgoing)
        {
            if (_incoming.TryGetValue(edge.To, out var toSet))
                toSet.Remove(edge);
        }

        _outgoing.Remove(taskId);
        _incoming.Remove(taskId);

        _tasks.Remove(taskId);

        return Result.OK;
    }


    public Result Delete(Func<TaskNode, bool> predicate)
    {
        if (TryFind(predicate, out TaskNode taskToDelete))
        {

            return Delete(taskToDelete);
        };
        return false;
    }

    public void AddRelation(TaskNode from, TaskNode to, EdgeType edgeType = EdgeType.Block)
    {
        var edge = new TaskEdge(){From = from.Id, To = to.Id, Type = edgeType};

        _outgoing.TryAdd(from.Id, new HashSet<TaskEdge>());
        _incoming.TryAdd(to.Id, new HashSet<TaskEdge>());

        _outgoing[from.Id].Add(edge);
        _incoming[to.Id].Add(edge);
        
        OnUpdate?.Invoke();
    }

    private IReadOnlyList<TaskNode> GetRelations(
        TaskNode task,
        IReadOnlyDictionary<Id, HashSet<TaskEdge>> edgeMap,
        Func<TaskEdge,Id> edgeId)
    {
        if (!edgeMap.TryGetValue(task.Id, out var edges) || edges.Count == 0)
            return [];

        var result = new List<TaskNode>();

        foreach (var edge in edges)
        {
            if (_tasks.TryGetValue(edgeId(edge), out var dep) 
                && dep.IsCompleted == false)
            {
                result.Add(dep);
            }
        }
        return result;
    }
 
    private IReadOnlyList<TaskNode> GetRecursiveRelations(
        TaskNode task,
        IReadOnlyDictionary<Id, HashSet<TaskEdge>> edgeMap,
        Func<TaskEdge, Id> nextIdSelector)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));
        if (!_tasks.ContainsKey(task.Id))
            throw new ArgumentException("Task not found in repository");

        var visited = new HashSet<Id>();
        var result = new List<TaskNode>();

        void Dfs(Id taskId)
        {
            if (!edgeMap.TryGetValue(taskId, out var edges))
                return;

            foreach (var edge in edges)
            {
                var nextId = nextIdSelector(edge);
                if (!visited.Add(nextId))
                    continue;

                if (_tasks.TryGetValue(nextId, out var nextTask)
                    && nextTask.IsCompleted == false)
                {
                    result.Add(nextTask);
                    Dfs(nextId);
                }
            }
        }

        Dfs(task.Id);
        return result;
    }
    public IEnumerator<TaskNode> GetEnumerator()
    {
        return  _tasks.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}