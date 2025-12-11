namespace UniversityLibrary.Domain.Entities
{
    public class Loan
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int BookId { get; set; }
        
        public Book? Book { get; set; }

        public bool IsActive() => Status == "Active";
        public bool IsReturned() => Status == "Returned";
        public bool CanBeReturned() => IsActive();
        public void ReturnLoan()
        {
            if (!CanBeReturned())
                throw new InvalidOperationException("El préstamo no puede ser devuelto porque ya está devuelto o no está activo");
            Status = "Returned";
            ReturnDate = DateTime.Now;
        }

        public void ActivateLoan()
        {
            Status = "Active";
            LoanDate = DateTime.Now;
            ReturnDate = null;
        }

        public bool IsOverdue(DateTime currentDate, int maxLoanDays = 30)
        {
            if (!IsActive()) return false;
            
            var dueDate = LoanDate.AddDays(maxLoanDays);
            return currentDate > dueDate;
        }

        public int GetDaysOverdue(DateTime currentDate, int maxLoanDays = 30)
        {
            if (!IsActive() || !IsOverdue(currentDate, maxLoanDays))
                return 0;
            var dueDate = LoanDate.AddDays(maxLoanDays);
            return (currentDate - dueDate).Days;
        }
    }
}