using UniversityLibrary.Application.DTOs.Book;

namespace UniversityLibrary.Application.Interfaces
{
    public interface ILoanService
    {
        Task<LoanDto?> GetByIdAsync(int id);
        Task<IEnumerable<LoanDto>> GetAllAsync();
        Task<LoanDto> CreateAsync(CreateLoanDto loanDto);
        
        Task<IEnumerable<LoanDto>> GetByBookIdAsync(int bookId);
        Task<IEnumerable<LoanDto>> GetByStudentNameAsync(string studentName);
        Task<IEnumerable<LoanDto>> GetActiveLoansAsync();
        Task<IEnumerable<LoanDto>> GetOverdueLoansAsync();
        Task<bool> CanBorrowBookAsync(int bookId);
        Task<int> CountActiveLoansByStudentAsync(string studentName);
    }
}