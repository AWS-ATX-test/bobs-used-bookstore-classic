using System.Collections.Generic;
using System.Data.Entity;
using Bookstore.Domain;

namespace Bookstore.Domain
{
    public enum ReferenceDataType
    {
        BookType,
        Condition,
        Genre,
        Publisher
    }
}

namespace Bookstore.Data
{
    public class BookstoreDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var referenceDataItems = new List<ReferenceDataItem> {
                new ReferenceDataItem { Id = 1 },
                new ReferenceDataItem { Id = 2 },
                new ReferenceDataItem { Id = 3 },

                new ReferenceDataItem { Id = 4 },
                new ReferenceDataItem { Id = 5 },
                new ReferenceDataItem { Id = 6 },
                new ReferenceDataItem { Id = 7 },

                new ReferenceDataItem { Id = 8 },
                new ReferenceDataItem { Id = 9 },
                new ReferenceDataItem { Id = 10 },
                new ReferenceDataItem { Id = 11 },
                new ReferenceDataItem { Id = 12 },
                new ReferenceDataItem { Id = 13 },
                new ReferenceDataItem { Id = 14 },

                new ReferenceDataItem { Id = 15 },
                new ReferenceDataItem { Id = 16 },
                new ReferenceDataItem { Id = 17 },
                new ReferenceDataItem { Id = 18 },
                new ReferenceDataItem { Id = 19 },
                new ReferenceDataItem { Id = 20 },
                new ReferenceDataItem { Id = 21 },
                new ReferenceDataItem { Id = 22 },
                new ReferenceDataItem { Id = 23 },
                new ReferenceDataItem { Id = 24 }
           };

            context.ReferenceData.AddRange(referenceDataItems);

            var books = new List<Book> {
                new Book { Id = 1 },
                new Book { Id = 2 },
                new Book { Id = 3 },
                new Book { Id = 4 },
                new Book { Id = 5 },
                new Book { Id = 6 },
                new Book { Id = 7 },
                new Book { Id = 8 }
            };

            // Set properties after creation
            books[0].Title = "2020: The Apocalypse";
            books[0].Author = "Li Juan";
            books[0].ISBN = "6556784356";

            books[1].Title = "Children Of Iron";
            books[1].Author = "Nikki Wolf";
            books[1].ISBN = "7665438976";

            books[2].Title = "Gold In The Dark";
            books[2].Author = "Richard Roe";
            books[2].ISBN = "5442280765";

            books[3].Title = "Leagues Of Smoke";
            books[3].Author = "Pat Candella";
            books[3].ISBN = "4556789542";

            books[4].Title = "Alone With The Stars";
            books[4].Author = "Carlos Salazar";
            books[4].ISBN = "4563358087";

            books[5].Title = "The Girl In The Polaroid";
            books[5].Author = "Terri Whitlock";
            books[5].ISBN = "2354435678";

            books[6].Title = "1001 Jokes";
            books[6].Author = "Mary Major";
            books[6].ISBN = "6554789632";

            books[7].Title = "My Search For Meaning";
            books[7].Author = "Mateo Jackson";
            books[7].ISBN = "4558786554";

            context.Book.AddRange(books);

            context.SaveChanges();
        }
    }
}