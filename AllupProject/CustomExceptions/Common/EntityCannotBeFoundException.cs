namespace AllupProject.CustomExceptions.Common;

public class EntityCannotBeFoundException : Exception
{
    public EntityCannotBeFoundException()
    {
    }

    public EntityCannotBeFoundException(string? message) : base(message)
    {
    }
}
