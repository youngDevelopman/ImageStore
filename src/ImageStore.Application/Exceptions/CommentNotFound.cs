namespace ImageStore.Application.Exceptions
{
    public class CommentNotFound: Exception
    {
        public CommentNotFound()
        {
        }

        public CommentNotFound(string message)
            : base(message)
        {
        }

        public CommentNotFound(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
