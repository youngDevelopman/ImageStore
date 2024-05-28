namespace ImageStore.API.Models
{
    public record GetPostsPaginatedRequest(int PageSize, string? Next, string? Previous);
}
