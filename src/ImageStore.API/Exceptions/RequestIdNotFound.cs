namespace ImageStore.API.Exceptions
{
    public class RequestIdNotFound : Exception
    {
        public RequestIdNotFound()
        {
        }

        public RequestIdNotFound(string message)
            : base(message)
        {
        }

        public RequestIdNotFound(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
