using ImageStore.API.Models;
using ImageStore.Application.Comments.Commands.AddComment;
using ImageStore.Application.Posts.Commands.RequestPost;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ImageStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [RequestSizeLimit(10906176)] // Set 101 MB, so we can have some space for Content field
        public async Task<IActionResult> AddPostRequest([FromForm] PostRequest request, CancellationToken cancellationToken)
        {
            using Stream imageFileStream = request.Image.OpenReadStream();
            string fileExtension = Path.GetExtension(request.Image.FileName);
            var result = await _mediator.Send(new RequestPostCommand(Guid.NewGuid(), request.Content, imageFileStream, fileExtension));

            return CreatedAtAction(nameof(AddPostRequest), result);
        }

        [HttpPost("{postId:Guid}/comments")]
        public async Task<IActionResult> AddCommentForPost(Guid postId, AddCommentRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new AddCommentCommand(postId, request.Content, Guid.NewGuid().ToString()));

            return CreatedAtAction(nameof(AddPostRequest), result);
        }
    }
}
