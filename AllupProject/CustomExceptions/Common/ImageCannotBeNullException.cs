namespace AllupProject.CustomExceptions.Common;

public class ImageCannotBeNullException:Exception
{
    public string Property { get; set; }
    public ImageCannotBeNullException()
    {
    }

    public ImageCannotBeNullException(string? message) : base(message)
    {
    }
    public ImageCannotBeNullException(string? message, string prop) : base(message)
    {
        Property = prop;
    }
}
