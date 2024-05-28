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

        public async Task<RequestPostResultDto> Handle(RequestPostCommand request, CancellationToken cancellationToken)
        {
            var fileName = $"{request.UserId}-{Guid.NewGuid()}{request.fileExtension}";

            var postRequest = new PostRequest()
            {
                UserId = request.UserId,
                Data = new PostRequestData()
                {
                    Caption = request.Content,
                    Creator = request.UserId.ToString(),
                    Image = fileName
                }
            };

            await _postRepository.AddPostRequestAsync(postRequest, cancellationToken);

            var metadata = new Dictionary<string, string>()
            {
                {"post-request-id", postRequest.Id.ToString() }
            };

            await _imageStorage.UploadFileAsync(request.File, fileName, metadata, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RequestPostResultDto(postRequest.Id);
        }
    }
}
