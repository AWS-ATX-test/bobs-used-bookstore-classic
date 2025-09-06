using System.Data.Entity;
using System.Threading.Tasks;

namespace Bookstore.Data
{
    public class Customer
    {
        public int Id { get; set; }
        public string Sub { get; set; }
        // Add other properties as needed
    }
}

namespace Bookstore.Data.Repositories
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer);
        Task<Customer> GetAsync(int id);
        Task<Customer> GetAsync(string sub);
        Task SaveChangesAsync();
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CustomerRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task ICustomerRepository.AddAsync(Customer customer)
        {
            await Task.Run(() => dbContext.Customer.Add(customer));
        }

        async Task<Customer> ICustomerRepository.GetAsync(int id)
        {
            return await dbContext.Customer.FindAsync(id);
        }

        async Task<Customer> ICustomerRepository.GetAsync(string sub)
        {
            return await dbContext.Customer.SingleOrDefaultAsync(x => x.Sub == sub);
        }

        async Task ICustomerRepository.SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}