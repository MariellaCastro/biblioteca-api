using AutoMapper;
using UniversityLibrary.Application.DTOs.Loan;
using UniversityLibrary.Application.Interfaces;
using UniversityLibrary.Domain.Entities;
using UniversityLibrary.Domain.Exceptions;
using UniversityLibrary.Domain.Ports.Out;

namespace UniversityLibrary.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LoanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<LoanDto?> GetByIdAsync(int id)
        {
            var loan = await _unitOfWork.Loans.GetWithBookAsync(id);
            return loan == null ? null : _mapper.Map<LoanDto>(loan);
        }

        public async Task<IEnumerable<LoanDto>> GetAllAsync()
        {
            var loans = await _unitOfWork.Loans.GetAllAsync();
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<LoanDto> CreateAsync(CreateLoanDto loanDto)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(loanDto.BookId);
            if (book == null)
            {
                throw new NotFoundException("Libro", loanDto.BookId);
            }

            if (!book.CanBeBorrowed())
            {
                throw new DomainException($"No se puede prestar el libro '{book.Title}' porque no hay stock disponible.");
            }

            var hasActiveLoan = await _unitOfWork.Loans.HasActiveLoanAsync(loanDto.BookId, loanDto.StudentName);
            if (hasActiveLoan)
            {
                throw new DomainException($"El estudiante '{loanDto.StudentName}' ya tiene un préstamo activo de este libro.");
            }

            var loan = new Loan
            {
                BookId = loanDto.BookId,
                StudentName = loanDto.StudentName,
                LoanDate = DateTime.Now,
                Status = "Active"
            };

            book.DecreaseStock();

            await _unitOfWork.Books.UpdateAsync(book);
            var createdLoan = await _unitOfWork.Loans.CreateAsync(loan);
            await _unitOfWork.SaveChangesAsync();

            var loanWithBook = await _unitOfWork.Loans.GetWithBookAsync(createdLoan.Id);
            return _mapper.Map<LoanDto>(loanWithBook!);
        }

        public async Task<LoanDto> ReturnLoanAsync(int loanId)
        {
            var loan = await _unitOfWork.Loans.GetWithBookAsync(loanId);
            if (loan == null)
            {
                throw new NotFoundException("Préstamo", loanId);
            }

            if (!loan.IsActive())
            {
                throw new DomainException($"El préstamo ya ha sido devuelto.");
            }

            if (loan.Book != null)
            {
                loan.Book.IncreaseStock();
                await _unitOfWork.Books.UpdateAsync(loan.Book);
            }

            loan.ReturnLoan();

            var updatedLoan = await _unitOfWork.Loans.UpdateAsync(loan);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<LoanDto>(updatedLoan);
        }

        public async Task<IEnumerable<LoanDto>> GetByBookIdAsync(int bookId)
        {
            var loans = await _unitOfWork.Loans.GetByBookIdAsync(bookId);
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<IEnumerable<LoanDto>> GetByStudentNameAsync(string studentName)
        {
            var loans = await _unitOfWork.Loans.GetByStudentNameAsync(studentName);
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<IEnumerable<LoanDto>> GetActiveLoansAsync()
        {
            var loans = await _unitOfWork.Loans.GetActiveLoansAsync();
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync()
        {
            var loans = await _unitOfWork.Loans.GetOverdueLoansAsync(DateTime.Now);
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<bool> CanBorrowBookAsync(int bookId)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(bookId);
            return book != null && book.CanBeBorrowed();
        }

        public async Task<int> CountActiveLoansByStudentAsync(string studentName)
        {
            return await _unitOfWork.Loans.CountActiveLoansByStudentAsync(studentName);
        }
    }
}