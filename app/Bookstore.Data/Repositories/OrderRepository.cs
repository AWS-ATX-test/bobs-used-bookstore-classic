
using Bookstore.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Data.Repositories
{
public class OrderRepository
{
    public class OrderStatistics
    {
        public int PendingOrders { get; set; }
        public int PastDueOrders { get; set; }
        public int OrdersThisMonth { get; set; }
        public int OrdersTotal { get; set; }
    }

    public class OrderFilters
    {
        public OrderStatus? OrderStatusFilter { get; set; }
        public DateTime? OrderDateFromFilter { get; set; }
        public DateTime? OrderDateToFilter { get; set; }
    }

    private readonly ApplicationDbContext dbContext;

    public enum OrderStatus {
        Pending,
        Ordered    }

    public OrderRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

        public async Task AddAsync(Order order)
        {
            await Task.Run(() => dbContext.Order.Add(order));
        }

        public async Task<Order> GetAsync(int id)
        {
            return await dbContext.Order
                .Include(x => x.Customer)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Order> GetAsync(int id, string sub)
        {
            return await dbContext.Order.SingleOrDefaultAsync(x => x.Id == id && x.Customer.Sub == sub);
        }

        public async Task<IEnumerable<Book>> ListBestSellingBooksAsync(int count)
        {
            return await dbContext.OrderItem
                .GroupBy(x => x.BookId)
                .OrderByDescending(x => x.Count())
                .Select(x => x.FirstOrDefault().Book)
                .Take(count)
                .ToListAsync();
        }

        public async Task<OrderStatistics> GetStatisticsAsync()
        {
            var startOfMonth = GetStartOfMonth();

            return await dbContext.Order
                .GroupBy(x => 1)
                .Select(x => new OrderStatistics
                {
                    PendingOrders = x.Count(y => y.OrderStatus == OrderRepository.OrderStatus.Pending),
                    PastDueOrders = x.Count(y => y.OrderStatus == OrderRepository.OrderStatus.Ordered && y.DeliveryDate < DateTime.UtcNow),
                    OrdersThisMonth = x.Count(),
                    OrdersTotal = x.Count()
                }).SingleOrDefaultAsync();
 }

        private static DateTime GetStartOfMonth()
        {
            return new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        }

        public async Task<IPaginatedList<Order>> ListAsync(OrderFilters filters, int pageIndex, int pageSize)
        {
            var query = dbContext.Order.AsQueryable();

            if (filters.OrderStatusFilter.HasValue)
            {
                query = query.Where(x => x.OrderStatus == filters.OrderStatusFilter);
            }

            if (filters.OrderDateFromFilter.HasValue)
            {
                query = query.Where(x => x.CreatedOn >= filters.OrderDateFromFilter);
            }

            if (filters.OrderDateToFilter.HasValue)
            {
                var filterData = filters.OrderDateToFilter.Value.OneSecondToMidnight();
                query = query.Where(x => x.CreatedOn < filterData );
            }

            query = query
                .Include(x => x.Customer)
                .Include(x => x.OrderItems)
                .Include(x => x.OrderItems.Select(y => y.Book));

            var result = new PaginatedList<Order>(query, pageIndex, pageSize);

            await result.PopulateAsync();

            return result;
        }

        public async Task<IEnumerable<Order>> ListAsync(string sub)
        {
            return await dbContext.Order
                .Include(x => x.OrderItems)
                .Include(x => x.OrderItems.Select(y => y.Book))
                .Where(x => x.Customer.Sub == sub)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
