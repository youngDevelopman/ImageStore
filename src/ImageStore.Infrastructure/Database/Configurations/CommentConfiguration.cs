using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImageStore.Infrastructure.Database.Configurations
{
    internal class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired(true);

            // We probaby do not want to have comments that have too many characters
            builder.Property(x => x.Content)
                .HasMaxLength(100);
        }
    }
}
