using ImageStore.Application.IntegrationTests.WebFactories;
using ImageStore.Application.Posts.Queries.GetPaginatedPosts;
using ImageStore.Domain.Models;
using ImageStore.Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ImageStore.Application.IntegrationTests
{

    // TODO: Add more test cases
    public class PostPaginationTests : IClassFixture<IntegrationTestWebAppFactoryPaginationTestsSeed>, IDisposable
    {
        private readonly IntegrationTestWebAppFactoryPaginationTestsSeed _factory;

        private readonly IServiceScope _scope;
        protected readonly ISender Sender;
        protected readonly ApplicationDbContext DbContext;
        public PostPaginationTests(IntegrationTestWebAppFactoryPaginationTestsSeed factory)
        {
            _factory = factory;

            _scope = factory.Services.CreateScope();

            Sender = _scope.ServiceProvider.GetRequiredService<ISender>();

            DbContext = _scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();
        }

        [Fact]
        public async Task GetPaginatedPosts_PassPageSizeOnly_ShouldReturnExpectedPosts()
        {
            // Arrage
            var request = new GetPaginatedPostsQuery(6, null, null);

            // Act
            var result = await Sender.Send(request);

            // Assert

            /*
             
            Check response items
             
            It is a good practice to check all of the fields that are present in Dto model,
                but for the sake of brevity we would just check Id, Comment Count and UserName.
             
             */
            Assert.Collection(result.Items, 
                post =>
                {
                    Assert.Equal("978463d5-c043-41d3-a4fb-0c5893d8e91b", post.Id.ToString());
                    Assert.Equal("John Doe", post.UserName);
                    Assert.Equal(15, post.CommentsCount);
                }, 
                post =>
                {
                    Assert.Equal("a40660e6-92e2-4563-a2af-a965efdac837", post.Id.ToString());
                    Assert.Equal("Mark Johnson", post.UserName);
                    Assert.Equal(13, post.CommentsCount);
                },
                post =>
                {
                    Assert.Equal("9bfb9dba-0039-4ed7-a1c6-ad601c724b6e", post.Id.ToString());
                    Assert.Equal("John Doe", post.UserName);
                    Assert.Equal(11, post.CommentsCount);
                },
                post =>
                {
                    Assert.Equal("07218c3e-7d4e-4138-98c4-e0b047486a2d", post.Id.ToString());
                    Assert.Equal("John Doe", post.UserName);
                    Assert.Equal(10, post.CommentsCount);
                },
                post =>
                {
                    Assert.Equal("a8edab68-1c41-4424-8651-5d996566c2c6", post.Id.ToString());
                    Assert.Equal("Mark Johnson", post.UserName);
                    Assert.Equal(10, post.CommentsCount);
                },
                post =>
                {
                    Assert.Equal("54a0e7e9-64b2-4d88-b1e0-6770c53eb18a", post.Id.ToString());
                    Assert.Equal("Mark Johnson", post.UserName);
                    Assert.Equal(9, post.CommentsCount);
                }
            );


            // Check the start and end cursors
            Assert.NotNull(result.Start);
            Assert.NotNull(result.End);

            var startCursor = PostCursor.CreateFromBase64String(result.Start);
            var endCursor = PostCursor.CreateFromBase64String(result.End);

            Assert.Equal("978463d5-c043-41d3-a4fb-0c5893d8e91b", startCursor.PostId.ToString());
            Assert.Equal(15, startCursor.CommentCount);
            Assert.Equal(DateTime.Parse("2023-05-15 14:30:45"), startCursor.UpdatedAt);

            Assert.Equal("54a0e7e9-64b2-4d88-b1e0-6770c53eb18a", endCursor.PostId.ToString());
            Assert.Equal(9, endCursor.CommentCount);
            Assert.Equal(DateTime.Parse("2023-03-10 16:45:20"), endCursor.UpdatedAt);
        }

        public void Dispose()
        {
            _scope?.Dispose();
            DbContext?.Dispose();
        }
    }
}