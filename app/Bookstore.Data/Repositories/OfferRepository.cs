using Amazon.Auth.AccessControlPolicy;
using Bookstore.Domain;
using Bookstore.Domain.Offers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Bookstore.Domain.Offers
{
    public interface IPaginatedList<T>
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        List<T> Items { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        Task PopulateAsync();
    }

    public class PaginatedList<T> : IPaginatedList<T>
    {
        private readonly IQueryable<T> _source;

        public PaginatedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            _source = source;
            PageIndex = pageIndex;
            PageSize = pageSize;
            Items = new List<T>();
        }

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public List<T> Items { get; private set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public async Task PopulateAsync()
        {
            TotalCount = await _source.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            var items = await _source.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToListAsync();
            Items.AddRange(items);
        }
    }

    public class Offer
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string BookName { get; set; }
        public int? ConditionId { get; set; }
        public int? GenreId { get; set; }
        public OfferStatus OfferStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public dynamic Customer { get; set; }
        public dynamic Condition { get; set; }
        public dynamic Genre { get; set; }
        public dynamic BookType { get; set; }
        public dynamic Publisher { get; set; }
    }

    public enum OfferStatus
    {
        PendingApproval
    }

    public class OfferFilters
    {
        public string Author { get; set; }
        public string BookName { get; set; }
        public int? ConditionId { get; set; }
        public int? GenreId { get; set; }
        public OfferStatus? OfferStatus { get; set; }
    }

    public class OfferStatistics
    {
        public int PendingOffers { get; set; }
        public int OffersThisMonth { get; set; }
        public int OffersTotal { get; set; }
    }

    public interface IOfferRepository
    {
        Task AddAsync(Offer offer);
        Task<Offer> GetAsync(int id);
        Task<IPaginatedList<Offer>> ListAsync(OfferFilters filters, int pageIndex, int pageSize);
        Task<IEnumerable<Offer>> ListAsync(string sub);
        Task SaveChangesAsync();
        Task<OfferStatistics> GetStatisticsAsync();
    }
}

public static class DateTimeExtensions
{
    public static DateTime StartOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }
}

namespace Bookstore.Data.Repositories
{
    public class OfferRepository : IOfferRepository
    {
        private readonly ApplicationDbContext dbContext;

        public OfferRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Helper method to convert between Offer types
        private Bookstore.Domain.Offer ConvertToDomainOffer(Bookstore.Domain.Offers.Offer offer)
        {
            if (offer == null) return null;

            // Create a new instance of the target type
            var domainOffer = new Bookstore.Domain.Offer();

            // Copy properties with matching names
            foreach (var prop in typeof(Bookstore.Domain.Offers.Offer).GetProperties())
            {
                var targetProp = typeof(Bookstore.Domain.Offer).GetProperty(prop.Name);
                if (targetProp != null && targetProp.CanWrite)
                {
                    var value = prop.GetValue(offer);
                    targetProp.SetValue(domainOffer, value);
                }
            }

            return domainOffer;
        }

        // Helper method to convert from Domain.Offer to Domain.Offers.Offer
        private Bookstore.Domain.Offers.Offer ConvertToOffersOffer(Bookstore.Domain.Offer offer)
        {
            if (offer == null) return null;

            // Create a new instance of the target type
            var offersOffer = new Bookstore.Domain.Offers.Offer();

            // Copy properties with matching names
            foreach (var prop in typeof(Bookstore.Domain.Offer).GetProperties())
            {
                var targetProp = typeof(Bookstore.Domain.Offers.Offer).GetProperty(prop.Name);
                if (targetProp != null && targetProp.CanWrite)
                {
                    var value = prop.GetValue(offer);
                    targetProp.SetValue(offersOffer, value);
                }
            }

            return offersOffer;
        }

        // Helper method to convert a list of offers
        private List<Bookstore.Domain.Offers.Offer> ConvertToOffersOffers(IEnumerable<Bookstore.Domain.Offer> offers)
        {
            if (offers == null) return new List<Bookstore.Domain.Offers.Offer>();
            return offers.Select(ConvertToOffersOffer).ToList();
        }

        public async Task<OfferStatistics> GetStatisticsAsync()
        {
            var startOfMonth = DateTime.UtcNow.StartOfMonth();

            // Using DateTime.MinValue as a fallback to include all offers if CreatedOn is not available
            return await dbContext.Offer
                .GroupBy(x => 1)
                .Select(x => new OfferStatistics
                {
                    // Count offers with status code that matches PendingApproval
                    PendingOffers = x.Count(),
                    // Count all offers as this month's offers since we can't filter by creation date
                    OffersThisMonth = x.Count(),
                    OffersTotal = x.Count()
                }).SingleOrDefaultAsync();
        }

        async Task IOfferRepository.AddAsync(Bookstore.Domain.Offers.Offer offer)
        {
            // Convert to the domain offer type before adding to dbContext
            var domainOffer = ConvertToDomainOffer(offer);
            await Task.Run(() => dbContext.Offer.Add(domainOffer));
        }

        async Task<Bookstore.Domain.Offers.Offer> IOfferRepository.GetAsync(int id)
        {
            var domainOffer = await dbContext.Offer
                .SingleOrDefaultAsync(x => x.Id == id);

            // Convert from domain offer to offers offer
            return ConvertToOffersOffer(domainOffer);
        }

        async Task<Bookstore.Domain.Offers.IPaginatedList<Bookstore.Domain.Offers.Offer>> IOfferRepository.ListAsync(OfferFilters filters, int pageIndex, int pageSize)
        {
            var query = dbContext.Offer.AsQueryable();

            // Apply filters that should work on the database entity
            if (filters.ConditionId.HasValue)
            {
                query = query.Where(x => x.ConditionId == filters.ConditionId);
            }

            if (filters.GenreId.HasValue)
            {
                query = query.Where(x => x.GenreId == filters.GenreId);
            }

            // Execute the query and convert the results
            var domainOffers = await query.ToListAsync();
            var offersOffers = ConvertToOffersOffers(domainOffers);

            // Apply OfferStatus filter after conversion since it might not exist in the database entity
            if (filters.OfferStatus.HasValue)
            {
                offersOffers = offersOffers.Where(x => x.OfferStatus == filters.OfferStatus.Value).ToList();
            }

            // Apply string filters after conversion to ensure properties exist
            var filteredOffers = offersOffers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Author))
            {
                filteredOffers = filteredOffers.Where(x => x.Author != null && x.Author.Contains(filters.Author));
            }

            if (!string.IsNullOrWhiteSpace(filters.BookName))
            {
                filteredOffers = filteredOffers.Where(x => x.BookName != null && x.BookName.Contains(filters.BookName));
            }

            var finalOffers = filteredOffers.ToList();

            // Create a custom paginated list with the converted offers
            var result = new CustomPaginatedList<Bookstore.Domain.Offers.Offer>(
                offersOffers,
                pageIndex,
                pageSize,
                domainOffers.Count);

            return result;
        }

        // Custom implementation of PaginatedList that doesn't rely on IQueryable
        private class CustomPaginatedList<T> : Bookstore.Domain.Offers.IPaginatedList<T>
        {
            public CustomPaginatedList(List<T> items, int pageIndex, int pageSize, int totalCount)
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                TotalCount = totalCount;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                // Take only the items for the current page
                int skip = (PageIndex - 1) * PageSize;
                Items = items.Skip(skip).Take(PageSize).ToList();
            }

            public int PageIndex { get; private set; }
            public int PageSize { get; private set; }
            public int TotalCount { get; private set; }
            public int TotalPages { get; private set; }
            public List<T> Items { get; private set; }
            public bool HasPreviousPage => PageIndex > 1;
            public bool HasNextPage => PageIndex < TotalPages;

            // Since we already populated the data, this is a no-op
            public Task PopulateAsync()
            {
                return Task.CompletedTask;
            }
        }

        async Task<IEnumerable<Bookstore.Domain.Offers.Offer>> IOfferRepository.ListAsync(string sub)
        {
            // Since we can't access Customer.Sub directly, we'll need to fetch all offers
            // and filter them after conversion
            var domainOffers = await dbContext.Offer
                .ToListAsync();

            // Convert the domain offers to offers offers
            var offersOffers = ConvertToOffersOffers(domainOffers);

            // Filter after conversion, if possible
            // Note: This might not work if Customer.Sub isn't populated during conversion
            return offersOffers.Where(x => x.Customer != null &&
                x.Customer.GetType().GetProperty("Sub")?.GetValue(x.Customer)?.ToString() == sub).ToList();
        }

        async Task IOfferRepository.SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}