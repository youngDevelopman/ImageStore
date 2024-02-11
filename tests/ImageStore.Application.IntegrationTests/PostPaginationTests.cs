using ImageStore.Application.IntegrationTests.WebFactories;
using ImageStore.Application.Posts.Queries.GetPaginatedPosts;
using ImageStore.Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ImageStore.Application.IntegrationTests
{
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
            var request = new GetPaginatedPostsQuery(5, null, null);

            // Act
            var result = await Sender.Send(request);

            // Assert
            Assert.Collection(result.Items, 
                post =>
                {
                    Assert.Equal("John Doe", post.UserName);
                    Assert.Equal(10, post.CommentsCount);
                }, 
                post =>
                {
                    Assert.Equal("Mark Johnson", post.UserName);
                    Assert.Equal(5, post.CommentsCount);
                },
                post =>
                {
                    Assert.Equal("Mark Johnson", post.UserName);
                    Assert.Equal(3, post.CommentsCount);
                }
            );

            Assert.NotNull(result.Start);
            Assert.NotNull(result.End);
        }

        public void Dispose()
        {
            _scope?.Dispose();
            DbContext?.Dispose();
        }
    }
}