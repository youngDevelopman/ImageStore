using ImageStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImageStore.Infrastructure.Database.Configurations
{
    public class PostRequestConfiguration : IEntityTypeConfiguration<PostRequest>
    {
        public void Configure(EntityTypeBuilder<PostRequest> builder)
        {
            builder.OwnsOne(x => x.Data, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ToJson();
            });
        }
    }
}
