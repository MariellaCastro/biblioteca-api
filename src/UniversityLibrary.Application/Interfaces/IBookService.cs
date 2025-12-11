using UniversityLibrary.Application.DTOs.Book;

namespace UniversityLibrary.Application.Interfaces
{
    public interface IBookService
    {
        Task<BookDto?> GetByIdAsync(int id);
        Task<IEnumerable<BookDto>> GetAllAsync();
        Task<BookDto> CreateAsync(CreateBookDto bookDto);
        Task<bool> DeleteAsync(int id);
        
        Task<BookDto?> GetByISBNAsync(string isbn);
        Task<IEnumerable<BookDto>> GetByAuthorAsync(string author);
        Task<IEnumerable<BookDto>> GetByTitleAsync(string title);
        Task<IEnumerable<BookDto>> GetAvailableBooksAsync();
    }
}