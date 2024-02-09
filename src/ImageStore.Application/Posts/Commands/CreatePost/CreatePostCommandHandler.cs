using ImageStore.Domain.Enums;
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

            try
            {
                var postRequest = await _postRepository.GetPostRequestByIdAsync(request.requestId, cancellationToken);
                if(postRequest == null) 
                {
                    throw new PostRequestNotFoundException($"Post request with id {postRequest.Id} has not been found. ");
                }
                else if (postRequest.Status == PostRequestStatus.PostCreated)
                {
                    throw new PostAlreadyExistsException($"Post already exists. Post Id: {postRequest.PostId}");
                }

                var post = new Post()
                {
                    Caption = postRequest.Data.Caption,
                    UserId = postRequest.UserId,
                    Image = request.ImagePath.ToString(),
                };

                await _postRepository.AddPostAsync(post, cancellationToken);
                postRequest.Status = PostRequestStatus.PostCreated;
                postRequest.PostId = post.Id;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
