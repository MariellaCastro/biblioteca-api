namespace UniversityLibrary.Domain.Ports.Out
{
    public interface IBookRepository
    {
        Task<Book?> GetByISBNAsync(string isbn);
        Task<IEnumerable<Book>> GetByAuthorAsync(string author);
        Task<IEnumerable<Book>> GetByTitleAsync(string title);
        Task<IEnumerable<Book>> GetAvailableBooksAsync();
        Task<bool> ISBNExistsAsync(string isbn);
        Task<Book?> GetWithLoansAsync(int id);
    }
}