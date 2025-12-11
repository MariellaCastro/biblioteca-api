using Microsoft.EntityFrameworkCore;
using UniversityLibrary.Domain.Entities;
using UniversityLibrary.Domain.Ports.Out;
using UniversityLibrary.Infrastructure.Persistence.Context;

namespace UniversityLibrary.Infrastructure.Persistence.Repositories
{
    public class LoanRepository : Repository<Loan>, ILoanRepository
    {
        public LoanRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Loan>> GetByBookIdAsync(int bookId)
        {
            return await _dbSet
                .Where(l => l.BookId == bookId)
                .Include(l => l.Book)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetByStudentNameAsync(string studentName)
        {
            return await _dbSet
                .Where(l => l.StudentName.Contains(studentName))
                .Include(l => l.Book)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
        {
            return await _dbSet
                .Where(l => l.Status == "Active")
                .Include(l => l.Book)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetOverdueLoansAsync(DateTime currentDate)
        {
            var thirtyDaysAgo = currentDate.AddDays(-30);
            return await _dbSet
                .Where(l => l.Status == "Active" && l.LoanDate < thirtyDaysAgo)
                .Include(l => l.Book)
                .OrderBy(l => l.LoanDate)
                .ToListAsync();
        }

        public async Task<bool> HasActiveLoanAsync(int bookId, string studentName)
        {
            return await _dbSet
                .AnyAsync(l => l.BookId == bookId && 
                              l.StudentName == studentName && 
                              l.Status == "Active");
        }

        public async Task<int> CountActiveLoansByStudentAsync(string studentName)
        {
            return await _dbSet
                .CountAsync(l => l.StudentName == studentName && l.Status == "Active");
        }

        public async Task<Loan?> GetWithBookAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Loan>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(l => l.LoanDate >= startDate && l.LoanDate <= endDate)
                .Include(l => l.Book)
                .OrderBy(l => l.LoanDate)
                .ToListAsync();
        }
    }
}