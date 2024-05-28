# Describing the architectural approach that was implemented with regard to image upload

There are generally two ways of implementing the system requirements that are given - using Synchronous or Asynchronous approaches.

## Synchronous approach

<img src="https://github.com/youngDevelopman/ImageStore/blob/master/docs/images/photo_2024-02-08_23-22-31.jpg" width=70% height=70%>

1. User initiates a POST request to the API in order to create a new Post.
2. API then performs the following operations sequentially in the following order:
	- Upload original image to S3
	- Process and upload the transformed image to S3 and return image URL
	- Create a corresponding record in DB referencing the file created in the previous step
3. Response is sent back to the client

**Pros**
- Fairly simple architecture, that is easy to develop, debug, and log.
- Users could perform a cancellation at any time
- Consistency
- Relatively fast(if compared to the async approach)
  
**Cons**
- The whole post creation process might take a long time and considering the poor user connection it might result in a bad user experience.

## Asynchronous approach (Implemented approach)

The following architecture facilitates the asynchronous appoach of image processing by using REST API(ASP.NET Core), Message Brokers(SQS), Remote FileStorage(S3), Serverless Computing(Lambda) and Relational Database(MS SQL)

![image](https://github.com/youngDevelopman/ImageStore/assets/31933374/eca98dc3-821b-46fb-9b5d-3f4e9689511d)


Steps:
1) The user initiates a POST request to the API in order to create a new Post. API then performs a series of operations (2 and 3) 
2) API adds Post Data to the RequestedPosts table that acts as an intermediate table to store the post-related data while it is being processed.
3) API uploads an image to S3. After 2 and 3 have been performed the API will immediately return the response to the user indicating that the post creation process has been initiated.At this point user can be sure that the post will be eventually processed either successfully or not.
4) When the S3 bucket has finished uploading the image, the corresponding event will be raised to the SQS
5) Lambda will pick up the message, retrieve the image from S3, process it
6) Lambda uploads the processed image to S3
7) When the S3 bucket has finished uploading the processed image, the corresponding event will be raised to the SQS
8) API will listen to this queue and pick up the message
9) API will then perform an ACID transaction where the data from the corresponding RequestedPosts table goes to the Posts table and then can be marked as Successful or removed.

**Pros**
- User can use UI while the image are being processed
- Works fine for users with a bad internet connection since a lot of operations are performed asynchronously
- Processing and uploading of the image are performed by a separate unit(AWS Lambda) which in turn offloads the burden on API, thus increasing the throughput of the system itself.
- Decomposed nature of the solution allows us to scale different components

**Cons**
- Harder to develop, test, debug, and log.
- Since the data has to travel across different components for the processed image to be uploaded it might result in larger latency(compared to the synchronous approach), so the user might wait longer for the post to be created. 
- Possibility of the duplicate messages being raised by S3 into SQS. https://stackoverflow.com/questions/56772299/s3-notification-creates-multiple-events
- Possibility of the duplicate messages being while processing the message

### Dead letter messages

If there are any dead-letter messages present in the dead-letter queue they might be processed by the API one another service.

Reasons for dead letters to appear might be the following:

- Some exception was raised in Lambda while processing the message
- Messages in the queue have not been processed in a timely fashion
- While processing a message from the queue API might move the message to the DLQ because it could not process it(for example, the file lacks of request-id in the metadata).
