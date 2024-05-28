using MediatR;

namespace ImageStore.Application.Posts.Commands.CreatePost
{
    public record CreatePostCommand(Guid requestId, Uri ImagePath) : IRequest;
}
