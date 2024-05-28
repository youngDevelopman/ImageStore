# What needs to be implemented
- Add processing of Dead-Letter Messages
- Add logging
- Add more tests
- Add test coverage
- Add static code analysis tool (i.e SonarCube)
- Add CI/CD
- Add SQL indexes for columns that are part of the cursor
- Add websockets/long-polling mechanism, so the client can keep track of the status of the post while it is being processed
- Use Dapper for queries (need to be analyzed whether it is worth doing)
- Consider dynamically increasing the default ```MAX_ADD_COMMENT_ATTEMPS``` number to have more comment-adding attempts. For example, a user has a lot of subscribers, so when he has just added a new post a lot of people would be adding the comments concurrently, so we might want to increase the attempts number.  
- Consider using Application-managed concurrency tokens in order to update ```'Row Version'``` column only when CommentCount has been updated. Now, the concurrency token is database-managed which means that every time the Image Url, Caption, or every other field is updated, ```'Row Version'``` is updated too, which results in redundant ```DbUpdateConcurrency``` exceptions to occur.
- Consider using expressions to dynamically build condition statements for the 'Where' clause: https://github.com/youngDevelopman/ImageStore/blob/93afe66b1ac0ba8e231a250736e6f35241ab4842/src/ImageStore.Infrastructure/Database/PostPaginationQueryBuilder.cs#L76-L79
https://github.com/youngDevelopman/ImageStore/blob/93afe66b1ac0ba8e231a250736e6f35241ab4842/src/ImageStore.Infrastructure/Database/PostPaginationQueryBuilder.cs#L97-L100

There is a lot more to add, but other improvements especially regarding the architecture should be discussed separately and they could be based on [Back-of-the-envelope estimation](https://github.com/youngDevelopman/ImageStore/blob/master/docs/back-of-the-envelope-estimation.md) section.
