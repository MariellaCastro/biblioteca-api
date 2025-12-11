using Microsoft.EntityFrameworkCore;
using UniversityLibrary.Domain.Entities;
using UniversityLibrary.Domain.Ports.Out;
using UniversityLibrary.Infrastructure.Persistence.Context;

namespace UniversityLibrary.Infrastructure.Persistence.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<Book?> GetByISBNAsync(string isbn)
        {
            return await _dbSet
                .FirstOrDefaultAsync(b => b.ISBN == isbn);
        }

        public async Task<IEnumerable<Book>> GetByAuthorAsync(string author)
        {
            return await _dbSet
                .Where(b => b.Author.Contains(author))
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetByTitleAsync(string title)
        {
            return await _dbSet
                .Where(b => b.Title.Contains(title))
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
        {
            return await _dbSet
                .Where(b => b.Stock > 0)
                .ToListAsync();
        }

        public async Task<bool> ISBNExistsAsync(string isbn)
        {
            return await _dbSet
                .AnyAsync(b => b.ISBN == isbn);
        }

        public async Task<Book?> GetWithLoansAsync(int id)
        {
            return await _dbSet
                .Include(b => b.Loans)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}