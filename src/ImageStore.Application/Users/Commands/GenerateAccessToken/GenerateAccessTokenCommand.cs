using MediatR;

namespace ImageStore.Application.Users.Commands.GenerateAccessToken
{
    public record GenerateAccessTokenCommand(string Email, string Password) : IRequest<GenerateAccessTokenResultDto>;
}
