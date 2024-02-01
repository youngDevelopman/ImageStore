namespace ImageStore.Domain.Enums
{
    public enum PostRequestStatus
    {
        Requested, // Post has just been requested
        ImageProcessing, // Original image has been succesfully uploaded
        OriginalImageFailedToBeUploaded, // Original image has failed to be uploaded
        ImageFailedToBeProcessed, // Something went wrong with image processing
        PostCreated // Post has been succesfully created
    }
}
