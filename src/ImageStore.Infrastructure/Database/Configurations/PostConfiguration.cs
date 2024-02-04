using ImageStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImageStore.Infrastructure.Database.Configurations
{
    internal class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            // We probaby do not want to have captions that have too many characters
            builder.Property(x => x.Caption)
                .IsRequired(false)
                .HasMaxLength(100);
        }
    }
}
