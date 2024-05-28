using ImageStore.Domain.Interfaces;
using MediatR;

namespace ImageStore.Application.Posts.Commands.CreatePost
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand>
    {
        private readonly IImageStorage _imageStorage;
        public CreatePostCommandHandler(IImageStorage imageStorage)
        {
            _imageStorage = imageStorage;
        }

        public Task Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine("CreatePostCommandHandler");
            return Task.CompletedTask;
        }
    }
}
