using ImageStore.Domain.Entities;
using ImageStore.Domain.Interfaces;
using MediatR;

namespace ImageStore.Application.Posts.Commands.RequestPost
{
    public class RequestPostCommandHandler : IRequestHandler<RequestPostCommand, RequestPostResultDto>
    {
        private readonly IImageStorage _imageStorage;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RequestPostCommandHandler(IImageStorage imageStorage, IPostRepository postRepository, IUnitOfWork unitOfWork)
        {
            _imageStorage = imageStorage;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;

        }

        // TODO: Complete the implementation
        public async Task<RequestPostResultDto> Handle(RequestPostCommand request, CancellationToken cancellationToken)
        {
            await _imageStorage.UploadFileAsync(request.File, cancellationToken);

            // TODO: Add other fields
            var postRequest = new PostRequest()
            {
               Data = new PostRequestData()
               {
                   Caption = request.Content,
                   Creator = request.UserId.ToString(),
               }
            };

            await _postRepository.AddPostRequestAsync(postRequest, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return new RequestPostResultDto(postRequest.Id);
        }
    }
}
