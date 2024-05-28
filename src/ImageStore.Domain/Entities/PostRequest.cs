namespace ImageStore.Domain.Entities
{
    public class PostRequest : BaseEntity
    {
        public PostRequestData Data { get; set; }
        public string Status { get; set; }
        public string? FailureDetails { get; set; }
    }

    public class PostRequestData
    {
        public string Caption { get; set; }
        public string Image { get; set; }
        public string Creator { get; set; }
    }
}
