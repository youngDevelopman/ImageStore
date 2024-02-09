namespace ImageStore.Application.Comments.Commands.AddComment
{
    public record AddCommentCommand(Guid PostId, Guid UserId, string Content) : IRequest<AddedCommentResultDto>;
}
