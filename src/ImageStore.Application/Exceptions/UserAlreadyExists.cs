namespace ImageStore.Application.Exceptions
{
    public class UserAlreadyExists : Exception
    {
        public UserAlreadyExists()
        {
        }

        public UserAlreadyExists(string message)
            : base(message)
        {
        }

        public UserAlreadyExists(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
