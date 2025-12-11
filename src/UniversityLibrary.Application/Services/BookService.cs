using AutoMapper;
using UniversityLibrary.Application.DTOs.Book;
using UniversityLibrary.Application.Interfaces;
using UniversityLibrary.Domain.Entities;
using UniversityLibrary.Domain.Exceptions;
using UniversityLibrary.Domain.Ports.Out;


namespace UniversityLibrary.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BookDto?> GetByIdAsync(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            return book == null ? null : _mapper.Map<BookDto>(book);
        }

        public async Task<IEnumerable<BookDto>> GetAllAsync()
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<BookDto> CreateAsync(CreateBookDto bookDto)
        {
            var existingBook = await _unitOfWork.Books.GetByISBNAsync(bookDto.ISBN);
            if (existingBook != null)
            {
                throw new DomainException($"El ISBN '{bookDto.ISBN}' ya está registrado.");
            }

            if (bookDto.Stock < 0)
            {
                throw new DomainException("El stock no puede ser negativo.");
            }

            var book = _mapper.Map<Book>(bookDto);
            var createdBook = await _unitOfWork.Books.CreateAsync(book);
            await _unitOfWork.SaveChangesAsync();
            
            return _mapper.Map<BookDto>(createdBook);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _unitOfWork.Books.GetWithLoansAsync(id);
            if (book == null)
            {
                throw new NotFoundException("Libro", id);
            }

            bool hasActiveLoans = false;
            foreach (var loan in book.Loans)
            {
                if (loan.IsActive())
                {
                    hasActiveLoans = true;
                    break;
                }
            }

            if (hasActiveLoans)
            {
                throw new DomainException($"No se puede eliminar el libro porque tiene préstamos activos.");
            }

            var result = await _unitOfWork.Books.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<BookDto?> GetByISBNAsync(string isbn)
        {
            var book = await _unitOfWork.Books.GetByISBNAsync(isbn);
            return book == null ? null : _mapper.Map<BookDto>(book);
        }

        public async Task<IEnumerable<BookDto>> GetByAuthorAsync(string author)
        {
            var books = await _unitOfWork.Books.GetByAuthorAsync(author);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<IEnumerable<BookDto>> GetByTitleAsync(string title)
        {
            var books = await _unitOfWork.Books.GetByTitleAsync(title);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<IEnumerable<BookDto>> GetAvailableBooksAsync()
        {
            var books = await _unitOfWork.Books.GetAvailableBooksAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }
    }
}