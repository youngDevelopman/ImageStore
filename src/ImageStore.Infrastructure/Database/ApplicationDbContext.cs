using ImageStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace ImageStore.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<PostRequest> PostRequests { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> ops) : base(ops)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }
    }
}
