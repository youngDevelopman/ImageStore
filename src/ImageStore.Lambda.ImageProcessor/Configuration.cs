namespace ImageStore.Lambda.ImageProcessor
{
    public class Configuration
    {
        public string BucketName { get; set; }
        public const string BucketNameKey = "Bucket_Name";

        public string OutputFolder { get; set; }
        public const string OutputFolderKey = "Output_Folder";

        public int ImageHeight { get; set; }
        public const string ImageHeightKey = "Image_Height";

        public int ImageWidth { get; set; }
        public const string ImageWidthKey = "Image_Width";

        public string ImageFormat { get; set; }
        public const string ImageFormatKey = "Image_Format";
    }
}
