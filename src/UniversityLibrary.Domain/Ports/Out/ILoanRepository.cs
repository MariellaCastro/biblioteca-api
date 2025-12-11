using UniversityLibrary.Domain.Entities;

namespace UniversityLibrary.Domain.Ports.Out
{
    public interface ILoanRepository : IRepository<Loan>
    {
        Task<IEnumerable<Loan>> GetByBookIdAsync(int bookId);
        Task<IEnumerable<Loan>> GetByStudentNameAsync(string studentName);
        Task<IEnumerable<Loan>> GetActiveLoansAsync();
        Task<IEnumerable<Loan>> GetOverdueLoansAsync(DateTime currentDate);
        Task<bool> HasActiveLoanAsync(int bookId, string studentName);
        Task<int> CountActiveLoansByStudentAsync(string studentName);
        Task<Loan?> GetWithBookAsync(int id);
        Task<IEnumerable<Loan>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}