namespace AllupProject.CustomExceptions.Common
{
    public class NameAlreadyExistException : Exception
    {
        public string PropertyName { get; set; }
        public NameAlreadyExistException() { }

        public NameAlreadyExistException(string? message) : base(message) { }

        public NameAlreadyExistException(string? propertyName, string? message) : base(message)
        {
            PropertyName = propertyName;
        }
    }
}
