using MediatR;

namespace ImageStore.Application.Posts.Commands.RequestPost
{
   public record RequestPostCommand(Guid UserId, string Content, Stream File) : IRequest<RequestPostResultDto>;
}
