namespace ImageStore.Application.Exceptions
{
    public class PostRequestNotFound : Exception
    {
        public PostRequestNotFound()
        {
        }

        public PostRequestNotFound(string message)
            : base(message)
        {
        }

        public PostRequestNotFound(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
