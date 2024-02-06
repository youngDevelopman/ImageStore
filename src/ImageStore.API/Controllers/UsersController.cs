using ImageStore.API.Models;
using ImageStore.Application.Users.Commands.GenerateAccessToken;
using ImageStore.Application.Users.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ImageStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserRequest registerUserRequest, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new RegisterUserCommand(registerUserRequest.UserName, registerUserRequest.Email, registerUserRequest.Password), 
                cancellationToken);

            return CreatedAtAction(nameof(RegisterUser), result);
        }

        [HttpPost("token")]
        public async Task<IActionResult> GenerateAccessToken(GenerateAccessTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GenerateAccessTokenCommand(request.Email, request.Password), cancellationToken);
            return Ok(result);
        }
    }
}
