# ImageStore

BandLab test challendge

## Tech stack
- ASP.NET Core
- AWS Lambda, S3, SQS
- MS SQL
- xUnit, Testcontainers, Docker

## Table of contents
- [Back-of-the-envelope estimation](https://github.com/youngDevelopman/ImageStore/blob/master/docs/back-of-the-envelope-estimation.md)
- [Architecture approach](https://github.com/youngDevelopman/ImageStore/blob/master/docs/architecture-approach.md)
- [Updating comment count](https://github.com/youngDevelopman/ImageStore/blob/master/docs/updating-comments-count.md)

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
