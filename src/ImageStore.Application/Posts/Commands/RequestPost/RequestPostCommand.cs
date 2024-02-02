using MediatR;

namespace ImageStore.Application.Posts.Commands.RequestPost
{
   public record RequestPostCommand(Guid userId, string content, Stream file) : IRequest;
}
