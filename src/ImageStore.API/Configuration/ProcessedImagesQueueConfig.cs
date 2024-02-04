namespace ImageStore.API.Configuration
{
    public class ProcessedImagesQueueConfig
    {
        public string SqsUrl { get; set; }
        public int MaxNumberOfMessages { get; set; }
        public int WaitTimeSeconds { get; set; }
    }

}
