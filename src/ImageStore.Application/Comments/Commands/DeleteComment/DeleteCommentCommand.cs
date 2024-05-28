namespace ImageStore.Application.Comments.Commands.DeleteComment
{
    public record DeleteCommentCommand(Guid UserId, Guid PostId, Guid CommentId) : IRequest;
}
