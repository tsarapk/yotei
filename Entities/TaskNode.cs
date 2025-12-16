using System;
using System.Collections.Generic;

namespace YoteiLib.Core;

public class TaskMeta
{
    public Actor CreatedBy { get; set; }
    public Actor PerfomedBy { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ModifiedAt { get; set; }
    
    public Result LastResult { get; set; }
}
public class TaskNode: IEntity
{
    public Id Id { get; init; }
    public TaskStatus Status { get; private set; }
    public int Priority { private set; get; } 
    public string Title { get; private set; }
    public string? Payload { get; private set; }
    public TaskMeta? Meta { get; private set; }
    public DateTimeOffset Deadline { get; private set; }
    public bool IsCompleted => Status == TaskStatus.Completed;

    private ResourceRepository _resources = new ResourceRepository();
    
    public TaskNode()
    {
        Meta = new TaskMeta();
        Id = new Id();
        
        Title = "Empty task";
        Meta.CreatedAt = DateTimeOffset.UtcNow;
        Meta.ModifiedAt = DateTimeOffset.UtcNow;
        Deadline = DateTimeOffset.MaxValue;
        Status = TaskStatus.Uncompleted;
    }

    internal TaskNode SetActor(Actor actor)
    {
        Meta.PerfomedBy = actor;
        SetStatus(TaskStatus.InProgress);
        return this;
    }

    public TaskNode SetPriority(int priority)
    {
        if (priority >= 0 && priority <= 10) // ToDO сделать чтобы можно было настраивать
        {
            Priority = priority;
            Meta.ModifiedAt = DateTimeOffset.UtcNow;
          
        }
        return this;
    }
    public TaskNode SetTitle(string title)
    {
        Title = title;
        Meta.ModifiedAt = DateTimeOffset.UtcNow;
        return this;
    }

    public TaskNode SetResource(Id id, IResourcesMap resourcesMap)
    {
        Resource res = resourcesMap.GetById(id);
        if(res == null) Meta.LastResult = Result.ResourceNotFound;
        if(!_resources.Add(res)) Meta.LastResult = Result.ResourceAlreadyExist;
        return this;
    }

    public TaskNode SetPayload(string payload)
    {
        Payload = payload;
        Meta.ModifiedAt = DateTimeOffset.UtcNow;
        return this;
    }

    internal TaskNode SetStatus(TaskStatus status)
    {
        Status = status;
        Meta.ModifiedAt = DateTimeOffset.UtcNow;
        return this;
    }
    public TaskNode SetStatusSecure(TaskStatus status)
    {
        if (status == TaskStatus.Completed) return this;
        return SetStatus(status);
    }
    public override string ToString()
    {
        return $"\nTitle: {Title} \nStatus: {Status} \nPriority: {Priority} \nDeadline: {Deadline} \nModified:{Meta.ModifiedAt} \nResources: {_resources}";
    }
}