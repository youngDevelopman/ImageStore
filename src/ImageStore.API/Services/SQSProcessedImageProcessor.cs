using Amazon.S3;
using Amazon.SQS;
using Amazon.SQS.Model;
using ImageStore.Infrastructure.Configuration;

namespace ImageStore.API.Services
{
    public class SQSProcessedImageProcessor : BackgroundService
    {
        private readonly IAmazonSQS _queue;
        public SQSProcessedImageProcessor(AwsCredentialsConfuguration awsCredentialsConfuguration)
        {
            _queue = new AmazonSQSClient(awsCredentialsConfuguration.Credentials, awsCredentialsConfuguration.Region);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"getting messages from the queue {DateTime.Now}");
                var request = new ReceiveMessageRequest()
                {
                    QueueUrl = "https://sqs.eu-central-1.amazonaws.com/648524383514/ImageStore-ProcessedImages-Queue",
                    MaxNumberOfMessages = 3,
                    WaitTimeSeconds = 5, // This parameter enables long polling for SQS
                };

                var response = await _queue.ReceiveMessageAsync(request);
                foreach (var message in response.Messages)
                {
                    Console.WriteLine(message.Body);
                    if (message.Body.Contains("Exception")) continue;

                    //await _queue.DeleteMessageAsync(
                        //"https://sqs.eu-central-1.amazonaws.com/648524383514/ImageStore-ProcessedImages-Queue", message.ReceiptHandle);
                }
            }
        }
    }
}
