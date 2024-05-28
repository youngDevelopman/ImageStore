using Microsoft.EntityFrameworkCore.Storage;

namespace ImageStore.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> CommitAsync(CancellationToken cancellationToken = default(CancellationToken));

    }
}
