# ImageStore

BandLab test challendge

## Tech stack
- ASP.NET Core
- AWS Lambda, S3, SQS
- MS SQL

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

# Get the next set post 
GET api/posts?pageSize=10&next=base64String

# Get the previous set post 
GET api/posts?pageSize=10&previous=base64String

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

