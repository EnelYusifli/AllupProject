namespace AllupProject.CustomExceptions.Common;

public class MoreThanMaxLengthException : Exception
{
    public string Property { get; set; }
    public MoreThanMaxLengthException()
    {
    }

    public MoreThanMaxLengthException(string? message) : base(message)
    {
    }

    public MoreThanMaxLengthException(string? message, string? prop) : base(message)
    {
        Property = prop;
    }
}
