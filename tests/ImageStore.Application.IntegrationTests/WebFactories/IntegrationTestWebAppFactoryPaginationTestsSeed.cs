using ImageStore.Domain.Entities;
using ImageStore.Infrastructure.Database;

namespace ImageStore.Application.IntegrationTests.WebFactories
{
    /// <summary>
    /// Integration test factory that configures MS SQL Docker image and seeds the data specific for the pagination tests
    /// </summary>
    public class IntegrationTestWebAppFactoryPaginationTestsSeed : BasicIntegrationTestWebAppFactory
    {
        public override void Seed(ApplicationDbContext context)
        {
            // Add Users
            var users = new User[]
            {
                new User()
                {
                    Email = "john.doe@gmail.com",
                    UserName = "John Doe",
                    PasswordHash = "xxRcZh30OEKt9wKJYo0Da2SQBlfF07BYCbDykt5jfnU=",
                    PasswordSalt = "3K6WKx5CW5eHPjJwp4msivcRcAoV18U4Pyoa+FU3ICg=",
                },
                new User()
                {
                    Email = "mark.johnson@gmail.com",
                    UserName = "Mark Johnson",
                    PasswordHash = "xxRcZh30OEKt9wKJYo0Da2SQBlfF07BYCbDykt5jfnU=",
                    PasswordSalt = "3K6WKx5CW5eHPjJwp4msivcRcAoV18U4Pyoa+FU3ICg=",
                }
            };
            context.Users.AddRange(users);

            context.Posts.AddRange(
                new Post()
                {
                    UserId = users[1].Id,
                    Caption = "Great day",
                    CommentsCount = 3,
                    Image = "some url with a great day image on it",
                    CreatedAt = DateTime.Parse("2022-08-10 12:30:20"),
                    UpdatedAt = DateTime.Parse("2022-08-10 12:30:20"),
                },
                new Post()
                {
                    UserId = users[0].Id,
                    Caption = "Hello",
                    CommentsCount = 10,
                    Image = "some url",
                    CreatedAt = DateTime.Parse("2022-01-06 14:45:20"),
                    UpdatedAt = DateTime.Parse("2022-01-06 14:45:20"),
                },
                new Post()
                {
                    UserId = users[1].Id,
                    Caption = "Flashing lights",
                    CommentsCount = 5,
                    Image = "flashing_lights.jpg",
                    CreatedAt = DateTime.Parse("2023-11-12 8:10:20"),
                    UpdatedAt = DateTime.Parse("2023-11-12 8:10:20"),
                }
            );

            context.SaveChanges();
        }
    }
}
