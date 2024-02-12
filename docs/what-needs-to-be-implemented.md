# What needs to be implemented
- Add processing of Dead-Letter Messages
- Add logging
- Add more tests
- Add test coverage
- Add static code analysis tool (i.e SonarCube)
- Add CI/CD
- Add SQL indexes for columns that are part of the cursor
- Use Dapper for queries (need to be analyzed whether it is worth doing)
- Consider using expressions to dynamically build condition statements for the 'Where' clause: [https://github.com/youngDevelopman/ImageStore/blob/93afe66b1ac0ba8e231a250736e6f35241ab4842/src/ImageStore.Infrastructure/Database/PostPaginationQueryBuilder.cs#L77](https://github.com/youngDevelopman/ImageStore/blob/93afe66b1ac0ba8e231a250736e6f35241ab4842/src/ImageStore.Infrastructure/Database/PostPaginationQueryBuilder.cs#L76-L79)
[https://github.com/youngDevelopman/ImageStore/blob/93afe66b1ac0ba8e231a250736e6f35241ab4842/src/ImageStore.Infrastructure/Database/PostPaginationQueryBuilder.cs#L97](https://github.com/youngDevelopman/ImageStore/blob/93afe66b1ac0ba8e231a250736e6f35241ab4842/src/ImageStore.Infrastructure/Database/PostPaginationQueryBuilder.cs#L97-L100)

There is a lot more to add, but other improvements especially regarding architecture should be discussed separately and they could be based on [Back-of-the-envelope estimation](https://github.com/youngDevelopman/ImageStore/blob/master/docs/back-of-the-envelope-estimation.md) section.
