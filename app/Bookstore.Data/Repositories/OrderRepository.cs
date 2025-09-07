using Bookstore.Domain;
using Bookstore.Domain.Books;
using Bookstore.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Domain.Orders
{
    public class OrderStatistics
    {
        public int PendingOrders { get; set; }
        public int PastDueOrders { get; set; }
        public int OrdersThisMonth { get; set; }
        public int OrdersTotal { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public Address Address { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime DeliveryDate { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Ordered
    }

    public class Customer
    {
        public string Sub { get; set; }
    }

    public class Address
    {
    }
}

namespace Bookstore.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext dbContext;

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
                .Include(x => x.Address)
                .Include(x => x.OrderItems)
                .Include(x => x.OrderItems.Select(y => y.Book))
                .Include(x => x.OrderItems.Select(y => y.Book.BookType))
                .Include(x => x.OrderItems.Select(y => y.Book.Condition))
                .Include(x => x.OrderItems.Select(y => y.Book.Genre))
                .Include(x => x.OrderItems.Select(y => y.Book.Publisher))
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
            var startOfMonth = DateTime.UtcNow.StartOfMonth();

            return await dbContext.Order
                .GroupBy(x => 1)
                .Select(x => new OrderStatistics
                {
                    PendingOrders = x.Count(y => y.OrderStatus == OrderStatus.Pending),
                    PastDueOrders = x.Count(y => y.OrderStatus == OrderStatus.Ordered && y.DeliveryDate < DateTime.UtcNow),
                    OrdersThisMonth = x.Count(y => y.CreatedOn >= startOfMonth),
                    OrdersTotal = x.Count()
                }).SingleOrDefaultAsync();
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
