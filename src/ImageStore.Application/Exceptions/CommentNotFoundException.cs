namespace ImageStore.Application.Exceptions
{
    public class CommentNotFoundException: Exception
    {
        public CommentNotFoundException()
        {
        }

        public CommentNotFoundException(string message)
            : base(message)
        {
        }

        public CommentNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
