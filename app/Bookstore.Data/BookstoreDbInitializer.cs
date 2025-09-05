using System.Collections.Generic;
using System.Data.Entity;
using Bookstore.Domain.Books;

namespace Bookstore.Domain.ReferenceData
{
    public enum ReferenceDataType
    {
        BookType,
        Condition,
        Genre,
        Publisher
    }

    public class ReferenceDataItem
    {
        public int Id { get; set; }
        public ReferenceDataType Type { get; private set; }
        public string Name { get; private set; }

        public ReferenceDataItem(ReferenceDataType type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}

namespace Bookstore.Data
{
    public class BookstoreDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var referenceDataItems = new List<Domain.ReferenceData.ReferenceDataItem> {
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.BookType, "Hardcover") { Id = 1 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.BookType, "Trade Paperback") { Id = 2 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.BookType, "Mass Market Paperback") { Id = 3 },

                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Condition, "New") { Id = 4 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Condition, "Like New") { Id = 5 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Condition, "Good") { Id = 6 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Condition, "Acceptable") { Id = 7 },

                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Genre, "Biographies") { Id = 8 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Genre, "Children's Books") { Id = 9 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Genre, "History") { Id = 10 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Genre, "Literature & Fiction") { Id = 11 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Genre, "Mystery, Thriller & Suspense") { Id = 12 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Genre, "Science Fiction & Fantasy") { Id = 13 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Genre, "Travel") { Id = 14 },

                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Arcadia Books") { Id = 15 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Astral Publishing") { Id = 16 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Moonlight Publishing") { Id = 17 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Dreamscape Press") { Id = 18 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Enchanted Library") { Id = 19 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Fantasia House") { Id = 20 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Horizon Books") { Id = 21 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Infinity Press") { Id = 22 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Paradigm Publishing") { Id = 23 },
                new Domain.ReferenceData.ReferenceDataItem(Domain.ReferenceData.ReferenceDataType.Publisher, "Aurora Publishing") { Id = 24 }
           };

            context.ReferenceData.AddRange(referenceDataItems);

            var books = new List<Domain.Books.Book>();

            // Create Book objects with appropriate constructor and set Id separately
            var book1 = new Domain.Books.Book();
            book1.Id = 1;
            books.Add(book1);

            var book2 = new Domain.Books.Book();
            book2.Id = 2;
            books.Add(book2);

            var book3 = new Domain.Books.Book();
            book3.Id = 3;
            books.Add(book3);

            var book4 = new Domain.Books.Book();
            book4.Id = 4;
            books.Add(book4);

            var book5 = new Domain.Books.Book();
            book5.Id = 5;
            books.Add(book5);

            var book6 = new Domain.Books.Book();
            book6.Id = 6;
            books.Add(book6);

            var book7 = new Domain.Books.Book();
            book7.Id = 7;
            books.Add(book7);

            var book8 = new Domain.Books.Book();
            book8.Id = 8;
            books.Add(book8);

            context.Book.AddRange(books);

            context.SaveChanges();
        }
    }
}