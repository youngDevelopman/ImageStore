using ImageStore.Domain.Entities;
using ImageStore.Domain.Interfaces;
using ImageStore.Domain.Enums;
using MediatR;
using ImageStore.Application.Exceptions;

namespace ImageStore.Application.Posts.Commands.CreatePost
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPostRepository _postRepository;
        public CreatePostCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _postRepository = postRepository;
        }

        public async Task Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            // TODO: Move whole logic to repository method??
            try
            {
                var postRequest = await _postRepository.GetPostRequestByIdAsync(request.requestId, cancellationToken);
                if(postRequest == null) 
                {
                    throw new PostRequestNotFound($"Post request with id {postRequest.Id} has not been found. ");
                }
                else if (postRequest.Status == PostRequestStatus.PostCreated)
                {
                    throw new PostAlreadyExists($"Post already exists. Post Id: {postRequest.PostId}");
                }

                var post = new Post()
                {
                    Caption = postRequest.Data.Caption,
                    Creator = postRequest.Data.Creator,
                    Image = request.ImagePath.ToString(),
                };

                await _postRepository.AddPostAsync(post, cancellationToken);
                postRequest.Status = PostRequestStatus.PostCreated;

                await _unitOfWork.CommitAsync();

                await transaction.CommitAsync();
            }
            catch(Exception ex)
            {
                //TODO: Log the exception
                await transaction.RollbackAsync();
                throw;
            }
            Console.WriteLine("CreatePostCommandHandler");
        }
    }
}
