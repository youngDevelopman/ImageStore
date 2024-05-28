using ImageStore.API.Models;
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
        public async Task<IActionResult> AddPostRequest([FromForm] PostRequest request, CancellationToken cancellationToken)
        {
            using Stream imageFileStream = request.Image.OpenReadStream();
            var result = await _mediator.Send(new RequestPostCommand(Guid.NewGuid(), request.Content, imageFileStream));

            return CreatedAtAction(nameof(AddPostRequest), result);
        }
    }
}
