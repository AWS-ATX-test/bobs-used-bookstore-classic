using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Data.Repositories
{
    // Define Address class locally to resolve the missing reference
    public class Address
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public bool IsActive { get; set; }
    }

    public class Customer
    {
        public string Sub { get; set; }
    }

    public interface IAddressRepository
    {
        Task<Address> GetAsync(string sub, int id);
        Task<IEnumerable<Address>> ListAsync(string sub);
        Task AddAsync(Address address);
        Task DeleteAsync(string sub, int id);
        Task SaveChangesAsync();
    }

    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext dbContext;
        private DbSet<Address> Addresses => dbContext.Set<Address>();

        public AddressRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task IAddressRepository.DeleteAsync(string sub, int id)
        {
            var address = await Addresses.SingleOrDefaultAsync(x => x.Customer.Sub == sub && x.Id == id);

            if (address == null) return;

            address.IsActive = false;
        }

        async Task<Address> IAddressRepository.GetAsync(string sub, int id)
        {
            return await Addresses.SingleOrDefaultAsync(x => x.Customer.Sub == sub && x.Id == id && x.IsActive == true);
        }

        async Task<IEnumerable<Address>> IAddressRepository.ListAsync(string sub)
        {
            return await Addresses.Where(x => x.Customer.Sub == sub && x.IsActive == true).ToListAsync();
        }

        async Task IAddressRepository.AddAsync(Address address)
        {
            await Task.Run(() => Addresses.Add(address));
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}