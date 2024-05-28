﻿using Amazon.S3;
using Amazon.S3.Util;
using Amazon.SQS;
using Amazon.SQS.Model;
using ImageStore.API.Exceptions;
using ImageStore.Application.Posts.Commands.CreatePost;
using ImageStore.Infrastructure.Configuration;
using MediatR;

namespace ImageStore.API.Services
{
    public class SQSProcessedImageProcessor : BackgroundService
    {
        private readonly IAmazonSQS _queue;
        private readonly IServiceProvider _services;
        private readonly ILogger<SQSProcessedImageProcessor> _logger;
        public SQSProcessedImageProcessor(
            AwsCredentialsConfuguration awsCredentialsConfuguration, 
            IServiceProvider services, 
            ILogger<SQSProcessedImageProcessor> logger)
        {
            _queue = new AmazonSQSClient(awsCredentialsConfuguration.Credentials, awsCredentialsConfuguration.Region);
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"getting messages from the queue {DateTime.Now}");
                var request = new ReceiveMessageRequest()
                {
                    QueueUrl = "https://sqs.eu-central-1.amazonaws.com/648524383514/ImageStore-ProcessedImages-Queue", //TODO: retrieve from config
                    MaxNumberOfMessages = 3,
                    WaitTimeSeconds = 5, // This parameter enables long polling for SQS
                };

                var response = await _queue.ReceiveMessageAsync(request);
                if (response.Messages.Any()) // Posibility of getting 0 messages
                {
                    try
                    {
                        using var scope = _services.CreateScope();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var s3Client = scope.ServiceProvider.GetRequiredService<IAmazonS3>();
                        foreach (var message in response.Messages)
                        {
                            var s3Event = S3EventNotification.ParseJson(message.Body);
                            var records = s3Event.Records;
                            foreach (var record in records)
                            {
                                var filePath = record.S3.Object.Key;
                                var bucket = record.S3.Bucket.Name;
                                var region = record.AwsRegion;

                                var imageUrl = new Uri($"https://{bucket}.s3.{region}.amazonaws.com/{filePath}");

                                var fileMetadata = await s3Client.GetObjectMetadataAsync(bucket, filePath);

                                var requestIdStr = fileMetadata.Metadata["post-request-id"];
                                if(requestIdStr == null )
                                {
                                    throw new RequestIdNotFound($"Request Id not found for {imageUrl}");
                                }
                                var requestId = Guid.Parse(requestIdStr);

                                await mediator.Send(new CreatePostCommand(requestId, imageUrl));
                            }
                            Console.WriteLine(message.Body);
                            if (message.Body.Contains("Exception")) continue;

                            //await _queue.DeleteMessageAsync(
                            //"https://sqs.eu-central-1.amazonaws.com/648524383514/ImageStore-ProcessedImages-Queue", message.ReceiptHandle);
                        }
                    }
                    catch (RequestIdNotFound ex)
                    {
                        // TODO: Put message to DLQ and remove from queue
                        _logger.LogError(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($" Unknown error {ex.Message}");
                    }

                }
            }
        }
    }
}
