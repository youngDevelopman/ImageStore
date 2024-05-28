namespace ImageStore.Application.Exceptions
{
    public class PostAlreadyExistsException : Exception
    {
        public PostAlreadyExistsException()
        {
        }

        public PostAlreadyExistsException(string message)
            : base(message)
        {
        }

        public PostAlreadyExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
