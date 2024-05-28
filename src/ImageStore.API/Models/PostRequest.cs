namespace ImageStore.API.Models
{
    public record PostRequest(string Content, IFormFile Image);
}
