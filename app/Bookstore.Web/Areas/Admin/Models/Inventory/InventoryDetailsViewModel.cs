using Bookstore.Domain.Books;

namespace Bookstore.Web.Areas.Admin.Models.Inventory{
    public class InventoryDetailsViewModel
    {
        public InventoryDetailsViewModel() { }

        public InventoryDetailsViewModel(Book book)
        {
            Author = book.Author;
            BookType = book.BookType.ToString();
            Condition = book.Condition.ToString();
            CoverImageUrl = book.CoverImageUrl;
            Genre = book.Genre.ToString();
            Id = book.Id;
            ISBN = book.ISBN;
            Name = book.Name;
            Price = book.Price;
            Publisher = book.Publisher.ToString();
            Quantity = book.Quantity;
            Summary = book.Summary;
            Year = book.Year.GetValueOrDefault();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public int Year { get; set; }

        public string Publisher { get; set; }

        public string BookType { get; set; }

        public string Genre { get; set; }

        public string Condition { get; set; }

        public string CoverImageUrl { get; set; }

        public string Summary { get; set; }

        public string ISBN { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }

    public class Book
    {
        public string Author { get; set; }
        public BookType BookType { get; set; }
        public Condition Condition { get; set; }
        public string CoverImageUrl { get; set; }
        public Genre Genre { get; set; }
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Publisher Publisher { get; set; }
        public int Quantity { get; set; }
        public string Summary { get; set; }
        public int? Year { get; set; }
    }

    public enum BookType
    {
        Text,
        // Add other members as needed
    }

    public enum Condition
    {
        Text,
        // Add other members as needed
    }

    public enum Genre
    {
        Text,
        // Add other members as needed
    }

    public enum Publisher
    {
        Text,
        // Add other members as needed
    }
}
