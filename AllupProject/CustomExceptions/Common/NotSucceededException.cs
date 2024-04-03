namespace AllupProject.CustomExceptions.Common;

public class NotSucceededException:Exception
{
    public NotSucceededException()
    {
    }

    public NotSucceededException(string? message) : base(message)
    {
    }
}
