using System.Reflection;

namespace ImageStore.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<PostRequest> PostRequests { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }

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
