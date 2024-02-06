namespace ImageStore.Application.Exceptions
{
    public class InvalidPassword: Exception
    {
        public InvalidPassword()
        {
        }

        public InvalidPassword(string message)
            : base(message)
        {
        }

        public InvalidPassword(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
