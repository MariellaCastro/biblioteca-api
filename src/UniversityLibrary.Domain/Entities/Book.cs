namespace UniversityLibrary.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();

        public bool CanBeBorrowed() => Stock > 0;

        public void DecreaseStock()
        {
            if (Stock <= 0)
                throw new InvalidOperationException("No hay stock disponible para este libro");
            Stock--;
        }

        public void IncreaseStock() => Stock++;
        public bool HasAvailableStock() => Stock > 0;
        public int GetAvailableCopies() => Stock;
    }
}