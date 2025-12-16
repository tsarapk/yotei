namespace YoteiLib.Core;


public class Result
{
    public ResultCode Code { get; set; }
    private bool IsTrue => Code == ResultCode.OK;
    public Result(bool value)
    {
        if (value) Code = ResultCode.OK;
        else Code = ResultCode.IDKError;
    }

    public Result(ResultCode value)
    {
        Code = value;
    }
    public static explicit operator bool(Result r) => r.IsTrue;
    public static implicit operator Result(bool value) => new Result(value);
    
    public static bool operator true(Result r) => r.IsTrue;
    public static bool operator false(Result r) => !r.IsTrue;

    public override string ToString()
    {
        return Code.ToString();
    }

    public static bool operator !(Result r) => !r.IsTrue;
    
    //наверное это можно как-то по-другому сделать
    public static readonly Result OK = new Result(ResultCode.OK);
    public static readonly Result TaskNotFound = new Result(ResultCode.TaskNotFound);
    public static readonly Result ActorNotFound = new Result(ResultCode.ActorNotFound);
    public static readonly Result ResourceNotFound = new Result(ResultCode.ResourceNotFound);
    public static readonly Result NotEnoughPrivileges = new Result(ResultCode.NotEnoughPrivileges);
    public static readonly Result NotEnoughResources = new Result(ResultCode.NotEnoughResources);
    public static readonly Result WrongActor = new Result(ResultCode.WrongActor);
    public static readonly Result TaskAlreadyCompleted = new Result(ResultCode.TaskAlreadyCompleted);
    public static readonly Result ThereAreUncompletedTasks = new Result(ResultCode.ThereAreUncompletedTasks);
    public static readonly Result TaskAlreadyExist = new Result(ResultCode.TaskAlreadyExist);
    public static readonly Result ResourceAlreadyExist = new Result(ResultCode.ResourceAlreadyExist);
    public static readonly Result IDKError = new Result(ResultCode.IDKError);
    public static readonly Result ActorAlreadyExist = new Result(ResultCode.ActorAlreadyExists);
    public static readonly Result RoleNotFound = new Result(ResultCode.RoleNotFound);
    public static readonly Result RoleAlreadyExist = new Result(ResultCode.RoleAlreadyExist);
}
