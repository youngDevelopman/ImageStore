# ImageStore

What to build:
- A system that allows you to upload images and comment on them
- No frontend/UI is required

User stories (where the user is an API consumer):
- As a user, I should be able to create posts with images (1 post - 1 image)
- As a user, I should be able to set a text caption when I create a post
- As a user, I should be able to comment on a post
- As a user, I should be able to delete a comment (created by me) from a post
- As a user, I should be able to get the list of all posts along with the last 2 comments
on each post

Functional requirements:
- RESTful Web API (JSON)
- Maximum image size - 100MB
- Allowed image formats: .png, .jpg, .bmp.
- Save uploaded images in the original format
- Convert uploaded images to .jpg format and resize to 600x600
- Serve images only in .jpg format
- Posts should be sorted by the number of comments (desc)
- Retrieve posts via a cursor-based pagination

Non-functional requirements:
- Maximum response time for any API call except uploading image files - 50 ms
- Minimum throughput handled by the system - 100 RPS

## Tech stack
- ASP.NET Core
- AWS Lambda, S3, SQS
- MS SQL
- xUnit, Testcontainers, Docker

## Table of contents
- [Back-of-the-envelope estimation](https://github.com/youngDevelopman/ImageStore/blob/master/docs/back-of-the-envelope-estimation.md)
- [Architecture approach](https://github.com/youngDevelopman/ImageStore/blob/master/docs/architecture-approach.md)
- [Updating comment count](https://github.com/youngDevelopman/ImageStore/blob/master/docs/updating-comments-count.md)
- [What needs to be implemented](https://github.com/youngDevelopman/ImageStore/blob/master/docs/what-needs-to-be-implemented.md)

## API 

### Users
```py

# Register new user
POST api/users
{
    "Email": "test@gmail.com",
    "UserName": "user1",
    "Password": "admin"
}


# Get JWT access token
POST api/users/token
{
    "Email": "test123@gmail.com",
    "Password": "admin1111"
}

```
### Posts

```py
# Create a new post. Form-data request
POST api/posts
{
  Image: file path
  Content:the last one  
}

# Get the first set post 
GET api/posts?pageSize=10
{
    "start": "GkDFlkfetrR",
    "end": "EYfgvmnwEUWI...",
    "items": [...]
}

# Get the next set post by providing 'end' cursor of the last request 
GET api/posts?pageSize=10&next=EYfgvmnwEUWI...
{
    "start": "GkDFlkfetrR...",
    "end": "EYfgvmnwEUWI...",
    "items": [...]
}

# Get the previous post by providing 'start' cursor of the last request
GET api/posts?pageSize=10&previous=GkDFlkfetrR...
{
    ...
}
```
### Comments
```py

# Add a new comment
POST api/posts/{postId}/comments
{
  "Content": "My first comment"
}

# Add a comment
DELETE api/posts/{postId}/comments/{commentGuid}
{
  "Content": "My first comment"
}

```

### Swagger

You could also use Swagger to trigger the endpoints. It is available under ```/swagger``` route

![image](https://github.com/youngDevelopman/ImageStore/assets/31933374/4629bf33-14ba-44ce-802a-e0b7a8cd2b3b)



## Settings
### appSettings.Development.json
```py
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ImageStore;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "AWS": {
    "Profile": "ImageStore-User",
    "Region": "eu-central-1",
    "ProcessedImagesQueue": {
      "SqsUrl": "SQS-QUEUE-URL",
      "MaxNumberOfMessages": 3,
      "WaitTimeSeconds": 5
    },
    "S3Images": {
      "Bucket": "image-store-test-app",
      "OriginalImageFolder": "original"
    }
  }
```
### secrets.json
```py
"AWS": {
    "AccessKey": "KEY",
    "SecretKey": "SECRET_KEY"
  },
  "JWT": {
    "Key": "JWT_KEY", // 128 bit string
    "Issuer":  "Image Store"
  }
```
### Apply the EF Core migrations
Open cmd and navigate to src/ImageStore.API and run the following command
```py
dotnet ef database update
```
