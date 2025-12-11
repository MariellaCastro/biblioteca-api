using Microsoft.EntityFrameworkCore;
using UniversityLibrary.Infrastructure.Persistence.Configurations;

namespace UniversityLibrary.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Domain.Entities.Book> Books { get; set; }
        public DbSet<Domain.Entities.Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new LoanConfiguration());
        }
    }
}