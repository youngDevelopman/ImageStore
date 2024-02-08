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
