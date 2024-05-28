namespace ImageStore.Infrastructure.Database.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAdync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(x  => x.Email == email);
        }
    }
}
