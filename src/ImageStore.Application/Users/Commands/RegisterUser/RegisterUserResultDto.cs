namespace ImageStore.Application.Users.Commands.RegisterUser
{
    public record RegisterUserResultDto(Guid UserId, string Email, string UserName);
}
