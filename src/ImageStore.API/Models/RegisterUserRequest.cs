namespace ImageStore.API.Models
{
    public record class RegisterUserRequest(string UserName, string Email, string Password);
}