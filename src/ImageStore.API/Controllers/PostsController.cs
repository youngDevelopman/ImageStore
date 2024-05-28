using ImageStore.API.Models;
using ImageStore.Application.Comments.Commands.AddComment;
using ImageStore.Application.Comments.Commands.DeleteComment;
using ImageStore.Application.Posts.Commands.RequestPost;
using ImageStore.Application.Posts.Queries.GetPaginatedPosts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageStore.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPostsPaginated([FromQuery] GetPostsPaginatedRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPaginatedPostsQuery(request.PageSize, request.Next, request.Previous), cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        [RequestSizeLimit(10906176)] // Set 101 MB, so we can have some space for Content field
        public async Task<IActionResult> AddPostRequest([FromForm] PostRequest request, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if(userId == null)
            {
                return BadRequest("User Id is missing");
            }

            using Stream imageFileStream = request.Image.OpenReadStream();
            string fileExtension = Path.GetExtension(request.Image.FileName);
            var result = await _mediator.Send(new RequestPostCommand(Guid.Parse(userId), request.Content, imageFileStream, fileExtension));

            return CreatedAtAction(nameof(AddPostRequest), result);
        }

        [HttpPost("{postId:Guid}/comments")]
        public async Task<IActionResult> AddCommentForPost(Guid postId, AddCommentRequest request, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if(userId == null)
            {
                return BadRequest("User Id is missing");
            }

            var result = await _mediator.Send(new AddCommentCommand(postId, Guid.Parse(userId), request.Content));

            return CreatedAtAction(nameof(AddPostRequest), result);
        }

        [HttpDelete("{postId:Guid}/comments/{commentId:Guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPostsPaginated(Guid postId, Guid commentId, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User Id is missing");
            }

            await _mediator.Send(new DeleteCommentCommand(Guid.Parse(userId), postId, commentId), cancellationToken);

            return NoContent();
        }
    }
}
