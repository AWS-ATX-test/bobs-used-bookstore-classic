using Bookstore.Domain;
using System.Collections.Generic;
using System.Data.Entity;

namespace Bookstore.Data
{
    public enum ReferenceDataType
    {
        BookType,
        Condition,
        Genre,
        Publisher
    }

    public class BookstoreDbInitializer1 : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var referenceDataItems = new List<ReferenceDataItem> {
                new ReferenceDataItem() { Id = 1 },
                new ReferenceDataItem() { Id = 2 },
                new ReferenceDataItem() { Id = 3 },

                new ReferenceDataItem() { Id = 4 },
                new ReferenceDataItem() { Id = 5 },
                new ReferenceDataItem() { Id = 6 },
                new ReferenceDataItem() { Id = 7 },

                new ReferenceDataItem() { Id = 8 },
                new ReferenceDataItem() { Id = 9 },
                new ReferenceDataItem() { Id = 10 },
                new ReferenceDataItem() { Id = 11 },
                new ReferenceDataItem() { Id = 12 },
                new ReferenceDataItem() { Id = 13 },
                new ReferenceDataItem() { Id = 14 },

                new ReferenceDataItem() { Id = 15 },
                new ReferenceDataItem() { Id = 16 },
                new ReferenceDataItem() { Id = 17 },
                new ReferenceDataItem() { Id = 18 },
                new ReferenceDataItem() { Id = 19 },
                new ReferenceDataItem() { Id = 20 },
                new ReferenceDataItem() { Id = 21 },
                new ReferenceDataItem() { Id = 22 },
                new ReferenceDataItem() { Id = 23 },
                new ReferenceDataItem() { Id = 24 }
           };

            context.ReferenceData.AddRange(referenceDataItems);

            var books = new List<Book> {
                new Book() { Id = 1 },
                new Book() { Id = 2 },
                new Book() { Id = 3 },
                new Book() { Id = 4 },
                new Book() { Id = 5 },
                new Book() { Id = 6 },
                new Book() { Id = 7 },
                new Book() { Id = 8 }
            };

            context.Book.AddRange(books);

            context.SaveChanges();
        }
    }
}
