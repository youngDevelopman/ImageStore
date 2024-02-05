namespace ImageStore.Application.Exceptions
{
    public class PostNotFound : Exception 
    {
        public PostNotFound()
        {
        }

        public PostNotFound(string message)
            : base(message)
        {
        }

        public PostNotFound(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
