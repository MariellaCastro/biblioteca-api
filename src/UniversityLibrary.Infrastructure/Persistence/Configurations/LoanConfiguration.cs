using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityLibrary.Domain.Entities;

namespace UniversityLibrary.Infrastructure.Persistence.Configurations
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.ToTable("Loans");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.StudentName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(l => l.LoanDate)
                .IsRequired();

            builder.Property(l => l.ReturnDate)
                .IsRequired(false);

            builder.Property(l => l.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(l => l.CreatedAt)
                .IsRequired();

            // Índices
            builder.HasIndex(l => l.BookId);
            builder.HasIndex(l => l.StudentName);
            builder.HasIndex(l => l.Status);
            builder.HasIndex(l => l.LoanDate);
        }
    }
}