namespace AllupProject.CustomExceptions.Common;

public class InvalidContentTypeException : Exception
{
    public string Property { get; set; }
    public InvalidContentTypeException()
    {
    }

    public InvalidContentTypeException(string? message) : base(message)
    {
    }
    public InvalidContentTypeException(string? message, string prop) : base(message)
    {
        Property = prop;
    }
}
