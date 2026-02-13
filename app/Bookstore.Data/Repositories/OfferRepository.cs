using Amazon.Auth.AccessControlPolicy;
using Bookstore.Domain;


using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Data.Repositories
{
    public enum OfferStatus
    {
        PendingApproval,
        Approved,
        Rejected
    }

    public class OfferStatistics
    {
        public int PendingOffers { get; set; }
        public int OffersThisMonth { get; set; }
        public int OffersTotal { get; set; }
    }

    public class OfferFilters
    {
        public string Author { get; set; }
        public string BookName { get; set; }
        public int? ConditionId { get; set; }
        public int? GenreId { get; set; }
        public OfferStatus? OfferStatus { get; set; }
    }

    public class OfferRepository
    {
        private readonly ApplicationDbContext dbContext;

        public OfferRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public static DateTime GetStartOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public async Task<OfferStatistics> GetStatisticsAsync()
        {
            var startOfMonth = GetStartOfMonth(DateTime.UtcNow);

            return await dbContext.Offer
                .GroupBy(x => 1)
                .Select(x => new OfferStatistics
                {
                    PendingOffers = x.Count(y => y.OfferStatus == OfferStatus.PendingApproval),
                    OffersThisMonth = x.Count(y => y.CreatedOn >= startOfMonth),
                    OffersTotal = x.Count()
                }).SingleOrDefaultAsync();
        }

        public async Task AddAsync(Offer offer)
        {
            await Task.Run(() => dbContext.Offer.Add(offer));
        }

        public Task<Offer> GetAsync(int id)
        {
            return dbContext.Offer.Include(x => x.Customer).SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IPaginatedList<Offer>> ListAsync(OfferFilters filters, int pageIndex, int pageSize)
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

        public async Task<IEnumerable<Offer>> ListAsync(string sub)
        {
            return await dbContext.Offer
                .Include(x => x.BookType)
                .Include(x => x.Genre)
                .Include(x => x.Condition)
                .Include(x => x.Publisher)
                .Where(x => x.Customer.Sub == sub)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
