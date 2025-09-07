using Bookstore.Domain;
using Bookstore.Domain.Books;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Domain.Books
{
    public class Book
    {
        public static readonly int LowBookThreshold = 5;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string CoverImageUrl { get; set; }
        public int GenreId { get; set; }
        public int PublisherId { get; set; }
        public int BookTypeId { get; set; }
        public int ConditionId { get; set; }

        public virtual Genre Genre { get; set; }
        public virtual Publisher Publisher { get; set; }
        public virtual BookType BookType { get; set; }
        public virtual Condition Condition { get; set; }
    }

    public class Genre
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class Publisher
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class BookType
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class Condition
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class BookFilters
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public int? ConditionId { get; set; }
        public int? BookTypeId { get; set; }
        public int? GenreId { get; set; }
        public int? PublisherId { get; set; }
        public bool LowStock { get; set; }
    }
}

namespace Bookstore.Data.Repositories
{
    public interface IBookRepository
    {
        Task<Book> GetAsync(int id);
        Task<IPaginatedList<Book>> ListAsync(BookFilters filters, int pageIndex, int pageSize);
        Task<IPaginatedList<Book>> ListAsync(string searchString, string sortBy, int pageIndex, int pageSize);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task SaveChangesAsync();
        Task<BookStatistics> GetStatisticsAsync();
    }

    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext dbContext;

        public BookRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task<Book> IBookRepository.GetAsync(int id)
        {
            return await dbContext.Book
                .Include("Genre")
                .Include("Publisher")
                .Include("BookType")
                .Include("Condition")
                .SingleAsync(x => x.Id == id);
        }

        async Task<IPaginatedList<Book>> IBookRepository.ListAsync(BookFilters filters, int pageIndex, int pageSize)
        {
            var query = dbContext.Book.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                query = query.Where(x => x.Name.Contains(filters.Name));
            }

            if (!string.IsNullOrWhiteSpace(filters.Author))
            {
                query = query.Where(x => x.Author.Contains(filters.Author));
            }

            if (filters.ConditionId.HasValue)
            {
                query = query.Where(x => x.ConditionId == filters.ConditionId);
            }

            if (filters.BookTypeId.HasValue)
            {
                query = query.Where(x => x.BookTypeId == filters.BookTypeId);
            }

            if (filters.GenreId.HasValue)
            {
                query = query.Where(x => x.GenreId == filters.GenreId);
            }

            if (filters.PublisherId.HasValue)
            {
                query = query.Where(x => x.PublisherId == filters.PublisherId);
            }

            if (filters.LowStock)
            {
                query = query.Where(x => x.Quantity <= Book.LowBookThreshold);
            }

            query = query
                .Include(x => x.Genre)
                .Include(x => x.Publisher)
                .Include(x => x.BookType)
                .Include(x => x.Condition);

            var result = new PaginatedList<Book>(query, pageIndex, pageSize);

            await result.PopulateAsync();

            return result;
        }

        async Task<IPaginatedList<Book>> IBookRepository.ListAsync(string searchString, string sortBy, int pageIndex, int pageSize)
        {
            var query = dbContext.Book.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(x => x.Name.Contains(searchString) ||
                                         x.Genre.Text.Contains(searchString) ||
                                         x.BookType.Text.Contains(searchString) ||
                                         x.ISBN.Contains(searchString) ||
                                         x.Publisher.Text.Contains(searchString));
            };

            switch (sortBy)
            {
                case "Name":
                    query = query.OrderBy(x => x.Name);
                    break;

                case "PriceAsc":
                    query = query.OrderBy(x => x.Price);
                    break;

                case "PriceDesc":
                    query = query.OrderByDescending(x => x.Price);
                    break;

                default:
                    query.OrderBy(x => x.Name);
                    break;
            }

            var result = new PaginatedList<Book>(query, pageIndex, pageSize);

            await result.PopulateAsync();

            return result;
        }

        async Task IBookRepository.AddAsync(Book book)
        {
            await Task.Run(() => dbContext.Book.Add(book));
        }

        async Task IBookRepository.UpdateAsync(Book book)
        {
            var existing = await dbContext.Book.FindAsync(book.Id);

            dbContext.Entry(existing).CurrentValues.SetValues(book);

            if (string.IsNullOrWhiteSpace(book.CoverImageUrl))
            {
                dbContext.Entry(existing).Property(x => x.CoverImageUrl).IsModified = false;
            }
        }

        async Task IBookRepository.SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        async Task<BookStatistics> IBookRepository.GetStatisticsAsync()
        {
            return await dbContext.Book
                .GroupBy(x => 1)
                .Select(x => new BookStatistics
                {
                    LowStock = x.Count(y => y.Quantity > 0 && y.Quantity < Book.LowBookThreshold),
                    OutOfStock = x.Count(y => y.Quantity == 0),
                    StockTotal = x.Count()
                }).SingleOrDefaultAsync();
        }
    }

    public class BookStatistics
    {
        public int LowStock { get; set; }
        public int OutOfStock { get; set; }
        public int StockTotal { get; set; }
    }

    public interface IPaginatedList<T>
    {
        Task PopulateAsync();
    }

    public class PaginatedList<T> : IPaginatedList<T>
    {
        public PaginatedList(IQueryable<T> query, int pageIndex, int pageSize)
        {
            // Implementation would be here
        }

        public Task PopulateAsync()
        {
            return Task.CompletedTask; // Placeholder implementation
        }
    }
}
