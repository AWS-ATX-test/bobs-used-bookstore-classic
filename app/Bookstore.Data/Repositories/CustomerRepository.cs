using Bookstore.Domain.Customers;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Bookstore.Data.Repositories
{
public class CustomerRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CustomerRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(Customer customer)
        {
            await Task.Run(() => dbContext.Customer.Add(customer));
        }

        public async Task<Customer> GetAsync(int id)
        {
            return await dbContext.Customer.FindAsync(id);
        }

        public async Task<Customer> GetAsync(string sub)
        {
            return await dbContext.Customer.SingleOrDefaultAsync(x => x.Sub == sub);
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
