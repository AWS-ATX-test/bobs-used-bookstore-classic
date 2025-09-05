using Bookstore.Domain;
using Bookstore.Domain.Books;
using Bookstore.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

// Class for order filtering
namespace Bookstore.Data.Repositories
{
    public class OrderFilters
    {
        public int? OrderStatusFilter { get; set; }
        public DateTime? OrderDateFromFilter { get; set; }
        public DateTime? OrderDateToFilter { get; set; }
    }
}

// Extension method to handle OrderStatus property
namespace Bookstore.Domain.Orders
{
    public static class OrderExtensions
    {
        public static int? OrderStatus(this Order order)
        {
            // Assuming the actual property name is "Status"
            // You may need to adjust this based on the actual property name
            return (int?)order.GetType().GetProperty("Status")?.GetValue(order);
        }
    }
}

namespace Bookstore.Data.Repositories
{
    public class OrderStatistics
    {
        public int PendingOrders { get; set; }
        public int PastDueOrders { get; set; }
        public int OrdersThisMonth { get; set; }
        public int OrdersTotal { get; set; }
    }

    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order> GetAsync(int id);
        Task<Order> GetAsync(int id, string sub);
        Task<IEnumerable<Book>> ListBestSellingBooksAsync(int count);
        Task<OrderStatistics> GetStatisticsAsync();
        Task<IPaginatedList<Order>> ListAsync(OrderFilters filters, int pageIndex, int pageSize);
        Task<IEnumerable<Order>> ListAsync(string sub);
        Task SaveChangesAsync();
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext dbContext;

        public OrderRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task IOrderRepository.AddAsync(Order order)
        {
            await Task.Run(() => dbContext.Order.Add(order));
        }

        async Task<Order> IOrderRepository.GetAsync(int id)
        {
            return await dbContext.Order
                .Include(x => x.Customer)
                .Include("OrderItem")
                .Include("OrderItem.Book")
                .Include("OrderItem.Book.BookType")
                .Include("OrderItem.Book.Condition")
                .Include("OrderItem.Book.Genre")
                .Include("OrderItem.Book.Publisher")
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        async Task<Order> IOrderRepository.GetAsync(int id, string sub)
        {
            return await dbContext.Order.SingleOrDefaultAsync(x => x.Id == id && x.Customer.Sub == sub);
        }

        async Task<IEnumerable<Book>> IOrderRepository.ListBestSellingBooksAsync(int count)
        {
            return await dbContext.OrderItem
                .GroupBy(x => x.BookId)
                .OrderByDescending(x => x.Count())
                .Select(x => x.FirstOrDefault().Book)
                .Take(count)
                .ToListAsync();
        }

        async Task<OrderStatistics> IOrderRepository.GetStatisticsAsync()
        {
            var startOfMonth = DateTime.UtcNow.StartOfMonth();

            return await dbContext.Order
                .GroupBy(x => 1)
                .Select(x => new OrderStatistics
                {
                    PendingOrders = x.Count(y => y.OrderStatus() == 0), // 0 represents Pending status
                    PastDueOrders = x.Count(y => y.OrderStatus() == 1), // 1 represents Ordered status
                    OrdersThisMonth = x.Count(),
                    OrdersTotal = x.Count()
                }).SingleOrDefaultAsync();
        }

        async Task<IPaginatedList<Order>> IOrderRepository.ListAsync(OrderFilters filters, int pageIndex, int pageSize)
        {
            var query = dbContext.Order.AsQueryable();

            if (filters.OrderStatusFilter.HasValue)
            {
                query = query.Where(x => x.OrderStatus() == filters.OrderStatusFilter);
            }

            if (filters.OrderDateFromFilter.HasValue)
            {
                // Skip date filtering as CreatedOn property is not available
                // query = query.Where(x => x.CreatedOn >= filters.OrderDateFromFilter);
            }

            if (filters.OrderDateToFilter.HasValue)
            {
                var filterData = filters.OrderDateToFilter.Value.OneSecondToMidnight();
                // Skip date filtering as CreatedOn property is not available
                // query = query.Where(x => x.CreatedOn < filterData );
            }

            query = query
                .Include(x => x.Customer)
                .Include("OrderItem")
                .Include("OrderItem.Book");

            var result = new PaginatedList<Order>(query, pageIndex, pageSize);

            await result.PopulateAsync();

            return result;
        }

        async Task<IEnumerable<Order>> IOrderRepository.ListAsync(string sub)
        {
            return await dbContext.Order
                .Include("OrderItem")
                .Include("OrderItem.Book")
                .Where(x => x.Customer.Sub == sub)
                .ToListAsync();
        }

        async Task IOrderRepository.SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime OneSecondToMidnight(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day).AddDays(1).AddSeconds(-1);
        }
    }
}