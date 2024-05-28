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
                .IsRequired(false);

            builder.OwnsOne(x => x.Data, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ToJson();
            });
        }
    }
}
