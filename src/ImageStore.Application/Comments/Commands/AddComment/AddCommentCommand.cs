using MediatR;

namespace ImageStore.Application.Comments.Commands.AddComment
{
    public record AddCommentCommand(Guid PostId, string Content, string UserId) : IRequest<AddedCommentResultDto>;
}
