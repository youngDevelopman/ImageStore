using ImageStore.Application.IntegrationTests.WebFactories;
using ImageStore.Application.Posts.Queries.GetPaginatedPosts;
using ImageStore.Domain.Models;
using ImageStore.Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ImageStore.Application.IntegrationTests
{

    // TODO: Add more test cases
    public class PostPaginationTests : IClassFixture<IntegrationTestWebAppFactoryPaginationTestsSeed>, IDisposable
    {
        private readonly IntegrationTestWebAppFactoryPaginationTestsSeed _factory;

        private readonly IServiceScope _scope;
        protected readonly ISender Sender;
        protected readonly ApplicationDbContext DbContext;

        private readonly ITestOutputHelper _output;
        public PostPaginationTests(IntegrationTestWebAppFactoryPaginationTestsSeed factory, ITestOutputHelper output)
        {
            _output = output;

            _factory = factory;

            _scope = factory.Services.CreateScope();

            Sender = _scope.ServiceProvider.GetRequiredService<ISender>();

            DbContext = _scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            DisplayCursorBase64StringForEveryItem();
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

        [Theory]
        [MemberData(nameof(PaginatedPostsMoveFromStartUntilEndInlineData))]
        [MemberData(nameof(PaginatedPostsMoveFromEndUntilStartInlineData))]
        public async Task GetPaginatedPosts_GetAllDataByMovingForwardAndBackwardsUsingCursors_ShouldReturnExpectedPosts(
            int pageSize, 
            string next, 
            string previous, 
            string expectedStart,
            string expectedEnd, 
            List<Guid> expectedIds)
        {
            //DisplayCursorBase64StringForEveryItem();

            // Arrange
            var request = new GetPaginatedPostsQuery(pageSize, next, previous);

            // Act
            var result = await Sender.Send(request);

            // Assert
            Assert.Equal(result.Items.Select(x => x.Id), expectedIds);
            Assert.Equal(result.Start, expectedStart);
            Assert.Equal(result.End, expectedEnd);
        }

        /*
         Containts array of input parameters that are used for testing of the pagination logic. 
         Starting from the first posts, each following array element has the next set of posts until in reaches the end.
        */
        public static IEnumerable<object[]> PaginatedPostsMoveFromStartUntilEndInlineData =>
             new List<object[]>
             {
                new object[] 
                { 
                    3, 
                    null, 
                    null, 
                    "OTc4NDYzZDUtYzA0My00MWQzLWE0ZmItMGM1ODkzZDhlOTFiXzE1XzUvMTUvMjAyMyAyOjMwOjQ1IFBN",
                    "OWJmYjlkYmEtMDAzOS00ZWQ3LWExYzYtYWQ2MDFjNzI0YjZlXzExXzEvNS8yMDIzIDc6MDA6MDAgUE0=",
                    new List<Guid>() 
                    { 
                        Guid.Parse("978463d5-c043-41d3-a4fb-0c5893d8e91b"), 
                        Guid.Parse("a40660e6-92e2-4563-a2af-a965efdac837"), 
                        Guid.Parse("9bfb9dba-0039-4ed7-a1c6-ad601c724b6e") 
                    } 
                },
                new object[]
                {
                    5,
                    "OWJmYjlkYmEtMDAzOS00ZWQ3LWExYzYtYWQ2MDFjNzI0YjZlXzExXzEvNS8yMDIzIDc6MDA6MDAgUE0=",
                    null,
                    "MDcyMThjM2UtN2Q0ZS00MTM4LTk4YzQtZTBiMDQ3NDg2YTJkXzEwXzEvNi8yMDIyIDI6NDU6MjAgUE0=",
                    "ZDA3MGNjNWYtMzk0Mi00MDc5LTgwYTctODU4NDMwNmRlOWI0XzdfNy8xMi8yMDIyIDEyOjAwOjMwIFBN",
                    new List<Guid>() 
                    { 
                        Guid.Parse("07218c3e-7d4e-4138-98c4-e0b047486a2d"), 
                        Guid.Parse("a8edab68-1c41-4424-8651-5d996566c2c6"), 
                        Guid.Parse("54a0e7e9-64b2-4d88-b1e0-6770c53eb18a"), 
                        Guid.Parse("24198357-4b1a-4d53-b800-0355eb20372d"),
                        Guid.Parse("d070cc5f-3942-4079-80a7-8584306de9b4"),
                    }
                },
                new object[]
                {
                    1,
                    "ZDA3MGNjNWYtMzk0Mi00MDc5LTgwYTctODU4NDMwNmRlOWI0XzdfNy8xMi8yMDIyIDEyOjAwOjMwIFBN",
                    null,
                    "YjQ1M2VhZTEtN2NkOC00ZjJkLThkYjYtMWRhNGZlYTA2Yjc5XzZfMTEvMjAvMjAyMiAxMDoyMDozMCBBTQ==",
                    "YjQ1M2VhZTEtN2NkOC00ZjJkLThkYjYtMWRhNGZlYTA2Yjc5XzZfMTEvMjAvMjAyMiAxMDoyMDozMCBBTQ==",
                    new List<Guid>()
                    {
                        Guid.Parse("b453eae1-7cd8-4f2d-8db6-1da4fea06b79")
                    }
                },
                new object[]
                {
                    10,
                    "YjQ1M2VhZTEtN2NkOC00ZjJkLThkYjYtMWRhNGZlYTA2Yjc5XzZfMTEvMjAvMjAyMiAxMDoyMDozMCBBTQ==",
                    null,
                    "Y2U1YTA3YTItZWJjMS00YTc3LThjZWItZDRkN2YxOTlmNmUxXzVfMTEvMTIvMjAyMyA4OjEwOjIwIEFN",
                    "NTYwOGZjYzEtOTZjYy00NjQwLTlhYzgtMWIyMjQ5ZjNjMWMwXzFfNS8xMC8yMDIyIDk6MTU6NDUgQU0=",
                    new List<Guid>()
                    {
                        Guid.Parse("ce5a07a2-ebc1-4a77-8ceb-d4d7f199f6e1"),
                        Guid.Parse("ff21b8ea-0fa7-4d4a-8b44-08dc34c330c1"),
                        Guid.Parse("48639671-6a35-48e4-b462-70582e69edae"),
                        Guid.Parse("c35546a6-a47d-42ca-aab3-cd2aa532cb94"),
                        Guid.Parse("5608fcc1-96cc-4640-9ac8-1b2249f3c1c0"),
                    }
                },
             };

        /*
        Containts array of input parameters that are used for testing of the pagination logic. 
        Starting from the last post, each following array element has the previous set of posts until it runs out of posts.
        */
        public static IEnumerable<object[]> PaginatedPostsMoveFromEndUntilStartInlineData =>
             new List<object[]>
             {
                                    //new object[] { "pageSize", "next", "previous", "expectedStart", "expectedEnd" , "expectedCount", "expected ids" },
                    new object[]
                    {
                        8,
                        null,
                        "NTYwOGZjYzEtOTZjYy00NjQwLTlhYzgtMWIyMjQ5ZjNjMWMwXzFfNS8xMC8yMDIyIDk6MTU6NDUgQU0=",
                        "NTRhMGU3ZTktNjRiMi00ZDg4LWIxZTAtNjc3MGM1M2ViMThhXzlfMy8xMC8yMDIzIDQ6NDU6MjAgUE0=",
                        "YzM1NTQ2YTYtYTQ3ZC00MmNhLWFhYjMtY2QyYWE1MzJjYjk0XzJfOS8xNS8yMDIyIDY6NDU6NTUgUE0=",
                        new List<Guid>()
                        {
                            Guid.Parse("54a0e7e9-64b2-4d88-b1e0-6770c53eb18a"),
                            Guid.Parse("24198357-4b1a-4d53-b800-0355eb20372d"),
                            Guid.Parse("d070cc5f-3942-4079-80a7-8584306de9b4"),
                            Guid.Parse("b453eae1-7cd8-4f2d-8db6-1da4fea06b79"),
                            Guid.Parse("ce5a07a2-ebc1-4a77-8ceb-d4d7f199f6e1"),
                            Guid.Parse("ff21b8ea-0fa7-4d4a-8b44-08dc34c330c1"),
                            Guid.Parse("48639671-6a35-48e4-b462-70582e69edae"),
                            Guid.Parse("c35546a6-a47d-42ca-aab3-cd2aa532cb94"),
                        }
                    },
                    new object[]
                    {
                        2,
                        null,
                        "NTRhMGU3ZTktNjRiMi00ZDg4LWIxZTAtNjc3MGM1M2ViMThhXzlfMy8xMC8yMDIzIDQ6NDU6MjAgUE0=",
                        "YThlZGFiNjgtMWM0MS00NDI0LTg2NTEtNWQ5OTY1NjZjMmM2XzEwXzEvNi8yMDIyIDI6NDU6MjAgUE0=",
                        "MDcyMThjM2UtN2Q0ZS00MTM4LTk4YzQtZTBiMDQ3NDg2YTJkXzEwXzEvNi8yMDIyIDI6NDU6MjAgUE0=",
                        new List<Guid>()
                        {
                            Guid.Parse("a8edab68-1c41-4424-8651-5d996566c2c6"),
                            Guid.Parse("07218c3e-7d4e-4138-98c4-e0b047486a2d"),
                        }
                    },
                    new object[]
                    {
                        5,
                        null,
                        "MDcyMThjM2UtN2Q0ZS00MTM4LTk4YzQtZTBiMDQ3NDg2YTJkXzEwXzEvNi8yMDIyIDI6NDU6MjAgUE0=",
                        "OTc4NDYzZDUtYzA0My00MWQzLWE0ZmItMGM1ODkzZDhlOTFiXzE1XzUvMTUvMjAyMyAyOjMwOjQ1IFBN",
                        "OWJmYjlkYmEtMDAzOS00ZWQ3LWExYzYtYWQ2MDFjNzI0YjZlXzExXzEvNS8yMDIzIDc6MDA6MDAgUE0=",
                        new List<Guid>()
                        {
                            Guid.Parse("978463d5-c043-41d3-a4fb-0c5893d8e91b"),
                            Guid.Parse("a40660e6-92e2-4563-a2af-a965efdac837"),
                            Guid.Parse("9bfb9dba-0039-4ed7-a1c6-ad601c724b6e"),
                        }
                    },
             };

        // Helper method that displays posts and their respective cursors. Used for debugging purposes.
        private void DisplayCursorBase64StringForEveryItem()
        {
            var posts = DbContext.Posts
                 .OrderByDescending(post => post.CommentsCount)
                 .ThenByDescending(post => post.UpdatedAt)
                 .ThenByDescending(post => post.Id)
                 .ToList();

            foreach (var post in posts)
            {
                var cursorStr = PostCursor.CreateFromPost(post).GetBase64Encoded();
                _output.WriteLine($"Post: {post.Id}, comment count: {post.CommentsCount},  cursor: {cursorStr}");
            }
        }

        public void Dispose()
        {
            _scope?.Dispose();
            DbContext?.Dispose();
        }
    }
}