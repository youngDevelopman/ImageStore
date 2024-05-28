using ImageStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImageStore.Infrastructure.Database.Configurations
{
    internal class PostRequestConfiguration : IEntityTypeConfiguration<PostRequest>
    {
        public void Configure(EntityTypeBuilder<PostRequest> builder)
        {
            builder
                .HasOne<Post>()
                .WithMany()
                .HasForeignKey(x => x.PostId)
                .IsRequired(false); // It it not required becuase we do not know post id in advance

            builder
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired(true);

            builder.OwnsOne(x => x.Data, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ToJson();
            });
        }
    }
}
