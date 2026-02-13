using Bookstore.Domain;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Data.Repositories
{
    public interface IPaginatedList<T> : IEnumerable<T>
    {
        int Count { get; }
        int PageIndex { get; }
        int PageSize { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        Task PopulateAsync();
    }

    public class ReferenceDataFilters
    {
        public int? ReferenceDataType { get; set; }
    }

    public class ReferenceDataRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ReferenceDataRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(ReferenceDataItem item)
        {
            await Task.Run(() => dbContext.ReferenceData.Add(item));
        }

        public async Task<ReferenceDataItem> GetAsync(int id)
        {
            return await dbContext.ReferenceData.FindAsync(id);
        }

        public async Task<IEnumerable<ReferenceDataItem>> FullListAsync()
        {
            return await dbContext.ReferenceData.ToListAsync();
        }

        public async Task<PaginatedList<ReferenceDataItem>> ListAsync(ReferenceDataFilters filters, int pageIndex, int pageSize)
        {
            var query = dbContext.ReferenceData.AsQueryable();

            if (filters.ReferenceDataType.HasValue)
            {
                query = query.Where(x => x.DataType == filters.ReferenceDataType.Value);
            }

            var result = new PaginatedList<ReferenceDataItem>(query, pageIndex, pageSize);

            await result.PopulateAsync();

            return result;
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
