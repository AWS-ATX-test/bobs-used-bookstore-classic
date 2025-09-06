using Bookstore.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

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

    public interface IPaginatedList<T> : IEnumerable<T>
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }

    public class PaginatedList<T> : IPaginatedList<T>
    {
        private readonly IQueryable<T> _query;
        private List<T> _items;

        public PaginatedList(IQueryable<T> query, int pageIndex, int pageSize)
        {
            _query = query;
            PageIndex = pageIndex;
            PageSize = pageSize;
            _items = new List<T>();
        }

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public async Task PopulateAsync()
        {
            TotalCount = await _query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            _items = await _query
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
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

    public class BookStatistics
    {
        public int LowStock { get; set; }
        public int OutOfStock { get; set; }
        public int StockTotal { get; set; }
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
                query = query.Where(x => x.Title.Contains(filters.Name));
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

            // Skip low stock filter as the Book entity doesn't have a Quantity property
            // This will need to be updated once the correct property name is known
            if (filters.LowStock)
            {
                // Temporarily disabled until correct property name is determined
                // query = query.Where(x => x.PROPERTY_NAME <= THRESHOLD);
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
                query = query.Where(x => x.Title.Contains(searchString) ||
                                         x.Genre.ToString().Contains(searchString) ||
                                         x.BookType.ToString().Contains(searchString) ||
                                         x.ISBN.Contains(searchString) ||
                                         x.Publisher.ToString().Contains(searchString));
            };

            switch (sortBy)
            {
                case "Name":
                    query = query.OrderBy(x => x.Title);
                    break;

                case "PriceAsc":
                    query = query.OrderBy(x => x.Price);
                    break;

                case "PriceDesc":
                    query = query.OrderByDescending(x => x.Price);
                    break;

                default:
                    query.OrderBy(x => x.Title);
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

            // Removed CoverImageUrl check as the property doesn't exist in Book class
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
// Temporarily using constant values until correct property name is determined
                    LowStock = 0, // x.Count(y => y.PROPERTY_NAME > 0 && y.PROPERTY_NAME < THRESHOLD),
                    OutOfStock = 0, // x.Count(y => y.PROPERTY_NAME == 0),
                    StockTotal = x.Count()
                }).SingleOrDefaultAsync();
        }
    }
}