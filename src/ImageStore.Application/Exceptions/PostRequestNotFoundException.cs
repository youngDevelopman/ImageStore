namespace ImageStore.Application.Exceptions
{
    public class PostRequestNotFoundException : Exception
    {
        public PostRequestNotFoundException()
        {
        }

        public PostRequestNotFoundException(string message)
            : base(message)
        {
        }

        public PostRequestNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
