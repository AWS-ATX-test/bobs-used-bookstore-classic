using Amazon.Auth.AccessControlPolicy;
using Bookstore.Domain;
using Bookstore.Domain.Offers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Data
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }
    }
}

namespace Bookstore.Domain.Offers
{
    public class OfferStatistics
    {
        public int PendingOffers { get; set; }
        public int OffersThisMonth { get; set; }
        public int OffersTotal { get; set; }
    }

    public class Offer
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public int? ConditionId { get; set; }
        public Condition Condition { get; set; }
        public int? GenreId { get; set; }
        public Genre Genre { get; set; }
        public OfferStatus OfferStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public Customer Customer { get; set; }
        public BookType BookType { get; set; }
        public Publisher Publisher { get; set; }
    }

    public enum OfferStatus
    {
        PendingApproval,
        Approved,
        Rejected
    }

    public class Customer
    {
        public string Sub { get; set; }
    }

    public class Genre
    {
    }

    public class Condition
    {
    }

    public class BookType
    {
    }

    public class Publisher
    {
    }
}

namespace Bookstore.Data.Repositories
{
    public interface IOfferRepository
    {
        Task AddAsync(Offer offer);
        Task<Offer> GetAsync(int id);
        Task<IPaginatedList<Offer>> ListAsync(OfferFilters filters, int pageIndex, int pageSize);
        Task<IEnumerable<Offer>> ListAsync(string sub);
        Task SaveChangesAsync();
        Task<OfferStatistics> GetStatisticsAsync();
    }

    public class OfferFilters
    {
        public string Author { get; set; }
        public string BookName { get; set; }
        public int? ConditionId { get; set; }
        public int? GenreId { get; set; }
        public OfferStatus? OfferStatus { get; set; }
    }

    public interface IPaginatedList<T>
    {
    }

    public class PaginatedList<T> : IPaginatedList<T>
    {
        public PaginatedList(IQueryable<T> query, int pageIndex, int pageSize)
        {
        }

        public Task PopulateAsync()
        {
            return Task.CompletedTask;
        }
    }

    public class OfferRepository : IOfferRepository
    {
        private readonly ApplicationDbContext dbContext;

        public OfferRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<OfferStatistics> GetStatisticsAsync()
        {
            var startOfMonth = DateTime.UtcNow.StartOfMonth();

            return await dbContext.Offer
                .GroupBy(x => 1)
                .Select(x => new OfferStatistics
                {
                    PendingOffers = x.Count(y => y.OfferStatus == OfferStatus.PendingApproval),
                    OffersThisMonth = x.Count(y => y.CreatedOn >= startOfMonth),
                    OffersTotal = x.Count()
                }).SingleOrDefaultAsync();
        }

        async Task IOfferRepository.AddAsync(Offer offer)
        {
            await Task.Run(() => dbContext.Offer.Add(offer));
        }

        Task<Offer> IOfferRepository.GetAsync(int id)
        {
            return dbContext.Offer.Include(x => x.Customer).SingleOrDefaultAsync(x => x.Id == id);
        }

        async Task<IPaginatedList<Offer>> IOfferRepository.ListAsync(OfferFilters filters, int pageIndex, int pageSize)
        {
            var query = dbContext.Offer.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Author))
            {
                query = query.Where(x => x.Author.Contains(filters.Author));
            }

            if (!string.IsNullOrWhiteSpace(filters.BookName))
            {
                query = query.Where(x => x.BookName.Contains(filters.BookName));
            }

            if (filters.ConditionId.HasValue)
            {
                query = query.Where(x => x.ConditionId == filters.ConditionId);
            }

            if (filters.GenreId.HasValue)
            {
                query = query.Where(x => x.GenreId == filters.GenreId);
            }

            if (filters.OfferStatus.HasValue)
            {
                query = query.Where(x => x.OfferStatus == filters.OfferStatus);
            }

            query = query.Include(x => x.Customer)
                .Include(x => x.Condition)
                .Include(x => x.Genre);



            var result = new PaginatedList<Offer>(query, pageIndex, pageSize);

            await result.PopulateAsync();

            return result;
        }

        async Task<IEnumerable<Offer>> IOfferRepository.ListAsync(string sub)
        {
            return await dbContext.Offer
                .Include(x => x.BookType)
                .Include(x => x.Genre)
                .Include(x => x.Condition)
                .Include(x => x.Publisher)
                .Where(x => x.Customer.Sub == sub)
                .ToListAsync();
        }

        async Task IOfferRepository.SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
