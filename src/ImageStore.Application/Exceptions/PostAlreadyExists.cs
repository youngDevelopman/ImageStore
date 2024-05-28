namespace ImageStore.Application.Exceptions
{
    public class PostAlreadyExists : Exception
    {
        public PostAlreadyExists()
        {
        }

        public PostAlreadyExists(string message)
            : base(message)
        {
        }

        public PostAlreadyExists(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
