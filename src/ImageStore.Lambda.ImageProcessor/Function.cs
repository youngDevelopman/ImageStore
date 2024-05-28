using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ImageStore.Lambda.ImageProcessor;

public class Function
{
    IAmazonS3 S3Client { get; set; }

    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        S3Client = new AmazonS3Client();
    }

    /// <summary>
    /// Constructs an instance with a preconfigured S3 client. This can be used for testing outside of the Lambda environment.
    /// </summary>
    /// <param name="s3Client"></param>
    public Function(IAmazonS3 s3Client)
    {
        this.S3Client = s3Client;
    }

    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
    /// to respond to S3 notifications.
    /// </summary>
    /// <param name="sqsEvent"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        try
        {
            var configuration = GetLambdaEnvironmentVariables();
            (string originalImageBucketName, string originalImageFileName) = GetImageInformationFromSQSEvent(sqsEvent);

            var request = new GetObjectRequest
            {
                BucketName = originalImageBucketName,
                Key = originalImageFileName
            };
            using var response = await S3Client.GetObjectAsync(request);

            using var resizedImageStream = new MemoryStream();
            ResizeImage(response.ResponseStream, resizedImageStream, configuration.ImageWidth, configuration.ImageHeight); 

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalImageFileName);
            var processedFilePath = $"{configuration.OutputFolder}/{fileNameWithoutExtension}.jpg";
            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = configuration.BucketName,
                Key = processedFilePath,
                InputStream = resizedImageStream,
            };

            await S3Client.PutObjectAsync(putObjectRequest);
        }
        catch(Exception ex)
        {
            context.Logger.LogLine($"Error processing message: {ex.Message}");
            context.Logger.LogError(ex.Message);
            context.Logger.LogError(ex.StackTrace);

            throw ex;
        }
    }

    public void ResizeImage(Stream inputStream, Stream resultStream, int width, int height)
    {
        var image = Image.Load(inputStream);
        image.Mutate(x => x.Resize(width: width, height: height));
        image.SaveAsJpeg(resultStream);
    }

    public (string bucketName, string fileName) GetImageInformationFromSQSEvent(SQSEvent sqsEvent)
    {
        var s3EventObj = S3EventNotification.ParseJson(sqsEvent.Records.First().Body);
        var s3Event = s3EventObj.Records.First();

        var bucketName = s3Event.S3.Bucket.Name;
        var objectKey = s3Event.S3.Object.Key;

        return (bucketName, objectKey);
    }

    public Configuration GetLambdaEnvironmentVariables()
    {
        var config = new Configuration();

        config.BucketName = Environment.GetEnvironmentVariable(Configuration.BucketNameKey) 
            ?? throw new ArgumentNullException($"{Configuration.BucketNameKey} environment variable is missing.");

        config.OutputFolder = Environment.GetEnvironmentVariable(Configuration.OutputFolderKey)
            ?? throw new ArgumentNullException($"{Configuration.OutputFolderKey} environment variable is missing.");

        config.ImageHeight = int.Parse(Environment.GetEnvironmentVariable(Configuration.ImageHeightKey) 
            ?? throw new ArgumentNullException($"{Configuration.ImageHeightKey} environment variable is missing.")
           );

        config.ImageWidth = int.Parse(Environment.GetEnvironmentVariable(Configuration.ImageWidthKey) 
            ?? throw new ArgumentNullException($"{Configuration.ImageWidthKey} environment variable is missing.")
           );

        return config;
    }
}