using Bookstore.Domain;
using Bookstore.Domain.Books;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Data.Repositories
{
    public static class DateTimeExtensions
    {
        public static DateTime OneSecondToMidnight(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, date.Kind);
        }
    }
    public enum OrderStatus
    {
        Pending,
        Ordered,
        Delivered,
        Cancelled
    }
    public class OrderStatistics
    {
        public int PendingOrders { get; set; }
        public int PastDueOrders { get; set; }
        public int OrdersThisMonth { get; set; }
        public int OrdersTotal { get; set; }
    }

    public class OrderFilters
    {
        public Nullable<int> OrderStatusFilter { get; set; }
        public DateTime? OrderDateFromFilter { get; set; }
        public DateTime? OrderDateToFilter { get; set; }
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
                .Include("Customer")
                .Include("OrderItems")
                .Include("OrderItems.Book")
                .Include("OrderItems.Book.BookType")
                .Include("OrderItems.Book.Condition")
                .Include("OrderItems.Book.Genre")
                .Include("OrderItems.Book.Publisher")
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
                .Select(x => dbContext.Set<Book>().Find(x.FirstOrDefault().BookId))
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
                    PendingOrders = x.Count(y => (int)OrderStatus.Pending == (int)OrderStatus.Pending),
                    PastDueOrders = x.Count(y => (int)OrderStatus.Ordered == (int)OrderStatus.Ordered),
                    OrdersThisMonth = x.Count(y => y.OrderDate >= startOfMonth),
                    OrdersTotal = x.Count()
                }).SingleOrDefaultAsync();
        }

        async Task<IPaginatedList<Order>> IOrderRepository.ListAsync(OrderFilters filters, int pageIndex, int pageSize)
        {
            var query = dbContext.Order.AsQueryable();

            // Skip OrderStatus filtering as the Order class doesn't have an OrderStatus property
            // You'll need to add an OrderStatus property to the Order class or modify the filtering logic
            if (filters.OrderStatusFilter.HasValue)
            {
                // Commented out due to missing OrderStatus property on Order class
                // query = query.Where(x => x.OrderStatus == filters.OrderStatusFilter);
            }

            if (filters.OrderDateFromFilter.HasValue)
            {
                query = query.Where(x => x.OrderDate >= filters.OrderDateFromFilter);
            }

            if (filters.OrderDateToFilter.HasValue)
            {
                var filterData = filters.OrderDateToFilter.Value.OneSecondToMidnight();
                query = query.Where(x => x.OrderDate < filterData );
            }

            query = query
                .Include("Customer")
                .Include("OrderItems")
                .Include("OrderItems.Book");

            var result = new PaginatedList<Order>(query, pageIndex, pageSize);

            await result.PopulateAsync();

            return result;
        }

        async Task<IEnumerable<Order>> IOrderRepository.ListAsync(string sub)
        {
            return await dbContext.Order
                .Include("OrderItems")
                .Include("OrderItems.Book")
                .Where(x => x.Customer.Sub == sub)
                .ToListAsync();
        }

        async Task IOrderRepository.SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}