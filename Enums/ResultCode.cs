namespace YoteiLib.Core;

public enum ResultCode
{
    OK,
    TaskNotFound,
    ActorNotFound,
    ActorAlreadyExists,
    ResourceNotFound,
    NotEnoughPrivileges,
    NotEnoughResources,
    WrongActor,
    TaskAlreadyCompleted,
    TaskAlreadyExist,
    ThereAreUncompletedTasks,
    ResourceAlreadyExist,
    IDKError, 
    RoleAlreadyExist,
    RoleNotFound
}