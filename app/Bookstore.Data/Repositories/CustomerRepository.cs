using System.Data.Entity;
using System.Threading.Tasks;
using Bookstore.Domain;

namespace Bookstore.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CustomerRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task ICustomerRepository.AddAsync(Models.Customer customer)
        {
            var domainCustomer = new Domain.Customer
            {
                Id = customer.Id,
                Sub = customer.Sub
            };
            await Task.Run(() => dbContext.Customer.Add(domainCustomer));
        }

        async Task<Models.Customer> ICustomerRepository.GetAsync(int id)
        {
            var domainCustomer = await dbContext.Customer.FindAsync(id);
            return domainCustomer == null ? null : MapToDomainModel(domainCustomer);
        }

        async Task<Models.Customer> ICustomerRepository.GetAsync(string sub)
        {
            var domainCustomer = await dbContext.Customer.SingleOrDefaultAsync(x => x.Sub == sub);
            return domainCustomer == null ? null : MapToDomainModel(domainCustomer);
        }

        async Task ICustomerRepository.SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        private Models.Customer MapToDomainModel(Domain.Customer domainCustomer)
        {
            return new Models.Customer
            {
                Id = domainCustomer.Id,
                Sub = domainCustomer.Sub
            };
        }
    }

    public interface ICustomerRepository
    {
        Task AddAsync(Models.Customer customer);
        Task<Models.Customer> GetAsync(int id);
        Task<Models.Customer> GetAsync(string sub);
        Task SaveChangesAsync();
    }
}

namespace Bookstore.Data.Repositories.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Sub { get; set; }
    }
}