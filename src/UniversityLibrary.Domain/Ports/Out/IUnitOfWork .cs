namespace UniversityLibrary.Domain.Ports.Out
{
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository Books { get; }
        ILoanRepository Loans { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}