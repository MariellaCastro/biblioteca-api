using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityLibrary.Domain.Entities;

namespace UniversityLibrary.Infrastructure.Persistence.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(b => b.Stock)
                .IsRequired();

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.HasIndex(b => b.ISBN)
                .IsUnique();

            builder.HasMany(b => b.Loans)
                .WithOne(l => l.Book)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}