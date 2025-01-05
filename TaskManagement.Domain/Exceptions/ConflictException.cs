namespace TaskManagement.Domain.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException() : base("There is a conflict with the current state of the resource.")
        {
        }

        public ConflictException(string message) : base(message)
        {
        }

        public ConflictException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
