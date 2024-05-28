namespace ImageStore.Domain.Entities
{
    public class PostRequest : BaseEntity
    {
        public Guid? PostId { get; set; }
        public Guid UserId { get; set; }
        public PostRequestData Data { get; set; }
        public PostRequestStatus Status { get; set; }
        public string? FailureDetails { get; set; }
    }

    public class PostRequestData
    {
        public string Caption { get; set; }
        public string Image { get; set; }
        public string Creator { get; set; }
    }
}
