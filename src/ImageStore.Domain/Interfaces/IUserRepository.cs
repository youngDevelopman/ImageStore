namespace ImageStore.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
        Task AddUserAdync(User user, CancellationToken cancellationToken);
    }
}
