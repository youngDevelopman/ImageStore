using MediatR;

namespace ImageStore.Application.Users.Commands.RegisterUser
{
    public record RegisterUserCommand(string UserName, string Email, string Password) : IRequest<RegisterUserResultDto>;
}
