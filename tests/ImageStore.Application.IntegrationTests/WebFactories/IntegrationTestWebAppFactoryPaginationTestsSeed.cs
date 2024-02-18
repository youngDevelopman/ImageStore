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
            // TODO: Move all this data to a separate JSON file and load it into the database

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

            // Add Posts
            context.Posts.AddRange(

                /*
                 Comment count order: 15, 13, 11, 10, 10, 9, 7, 6, 5, 3, 3, 2, 1 
                

                 Ids have to be set manually in order to check the right entity in the test cases
                */

                // First two have the same comment count, but different time
                new Post()
                {
                    Id = Guid.Parse("48639671-6a35-48e4-b462-70582e69edae"),
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
                    Caption = "Nice",
                    CommentsCount = 3,
                    Image = "nice",
                    CreatedAt = DateTime.Parse("2022-08-10 12:30:25"),
                    UpdatedAt = DateTime.Parse("2022-08-10 12:30:25"),
                },

                // Next two have the same comment count, time, but obviusly different ids. Sorting has to be performed based on id
                new Post()
                {
                    Id = Guid.Parse("07218c3e-7d4e-4138-98c4-e0b047486a2d"),
                    UserId = users[0].Id,
                    Caption = "Hello",
                    CommentsCount = 10,
                    Image = "some url",
                    CreatedAt = DateTime.Parse("2022-01-06 14:45:20"),
                    UpdatedAt = DateTime.Parse("2022-01-06 14:45:20"),
                },
                new Post()
                {
                    Id = Guid.Parse("a8edab68-1c41-4424-8651-5d996566c2c6"),
                    UserId = users[1].Id,
                    Caption = "A dancer dances under the starry night sky",
                    CommentsCount = 10,
                    Image = "dancer.jpg",
                    CreatedAt = DateTime.Parse("2022-01-06 14:45:20"),
                    UpdatedAt = DateTime.Parse("2022-01-06 14:45:20"),
                },

                // The posts below have different comment count, time and id 
                new Post()
                {
                    Id = Guid.Parse("24198357-4b1a-4d53-b800-0355eb20372d"),
                    UserId = users[0].Id,
                    Caption = "A dog chases a busy street.",
                    CommentsCount = 8,
                    Image = "dog.jpg",
                    CreatedAt = DateTime.Parse("2022-01-06 14:45:20"),
                    UpdatedAt = DateTime.Parse("2022-01-06 14:45:20"),
                },
                new Post()
                {
                    Id = Guid.Parse("ce5a07a2-ebc1-4a77-8ceb-d4d7f199f6e1"),
                    UserId = users[1].Id,
                    Caption = "Flashing lights",
                    CommentsCount = 5,
                    Image = "flashing_lights.jpg",
                    CreatedAt = DateTime.Parse("2023-11-12 8:10:20"),
                    UpdatedAt = DateTime.Parse("2023-11-12 8:10:20"),
                },
                new Post
                {
                    Id = Guid.Parse("5608fcc1-96cc-4640-9ac8-1b2249f3c1c0"),
                    UserId = users[1].Id,
                    Caption = "A chef cooks a delicious meal of the starry night sky.",
                    CommentsCount = 1,
                    Image = "chef.jpg",
                    CreatedAt = DateTime.Parse("2022-05-10 09:15:45"),
                    UpdatedAt = DateTime.Parse("2022-05-10 09:15:45")
                },
                new Post
                {
                    Id = Guid.Parse("d070cc5f-3942-4079-80a7-8584306de9b4"),
                    UserId = users[0].Id,
                    Caption = "A chef chases a quiet morning.",
                    CommentsCount = 7,
                    Image = "morning.jpg",
                    CreatedAt = DateTime.Parse("2022-07-12 12:00:30"),
                    UpdatedAt = DateTime.Parse("2022-07-12 12:00:30")
                },
                new Post
                {
                    Id = Guid.Parse("c35546a6-a47d-42ca-aab3-cd2aa532cb94"),
                    UserId = users[1].Id,
                    Caption = "A dancer jumps over the starry night sky.",
                    CommentsCount = 2,
                    Image = "sky.jpg",
                    CreatedAt = DateTime.Parse("2022-09-15 18:45:55"),
                    UpdatedAt = DateTime.Parse("2022-09-15 18:45:55")
                },
                new Post 
                {
                    Id = Guid.Parse("b453eae1-7cd8-4f2d-8db6-1da4fea06b79"),
                    UserId = users[1].Id,
                    Caption = "A painter captures the essence of early morning.", 
                    CommentsCount = 6, 
                    Image = "painter.jpg", 
                    CreatedAt = DateTime.Parse("2022-11-20 10:20:30"), 
                    UpdatedAt = DateTime.Parse("2022-11-20 10:20:30") 
                },
                new Post 
                {
                    Id = Guid.Parse("9bfb9dba-0039-4ed7-a1c6-ad601c724b6e"),
                    UserId = users[0].Id,
                    Caption = "A musician plays melodies under a full moon.", 
                    CommentsCount = 11, 
                    Image = "musician.jpg", 
                    CreatedAt = DateTime.Parse("2023-01-05 19:00:00"), 
                    UpdatedAt = DateTime.Parse("2023-01-05 19:00:00") 
                },
                new Post 
                {
                    Id = Guid.Parse("54a0e7e9-64b2-4d88-b1e0-6770c53eb18a"),
                    UserId = users[1].Id,
                    Caption = "A writer finds inspiration in the bustling city life.", 
                    CommentsCount = 9,
                    Image = "writer.jpg", 
                    CreatedAt = DateTime.Parse("2023-03-10 16:45:20"), 
                    UpdatedAt = DateTime.Parse("2023-03-10 16:45:20") 
                },
                new Post 
                {
                    Id = Guid.Parse("978463d5-c043-41d3-a4fb-0c5893d8e91b"),
                    UserId = users[0].Id,
                    Caption = "A photographer captures the fleeting moments of joy.", 
                    CommentsCount = 15, 
                    Image = "photographer.jpg",
                    CreatedAt = DateTime.Parse("2023-05-15 14:30:45"), 
                    UpdatedAt = DateTime.Parse("2023-05-15 14:30:45") 
                },
                new Post 
                {
                    Id = Guid.Parse("a40660e6-92e2-4563-a2af-a965efdac837"),
                    UserId = users[1].Id,
                    Caption = "A traveler shares tales from distant lands.", 
                    CommentsCount = 13, 
                    Image = "traveler.jpg", 
                    CreatedAt = DateTime.Parse("2023-07-20 12:15:30"), 
                    UpdatedAt = DateTime.Parse("2023-07-20 12:15:30") }
                );

            context.SaveChanges();
        }
    }
}
