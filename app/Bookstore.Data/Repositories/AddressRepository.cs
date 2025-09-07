using Bookstore.Domain.Addresses;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Data.Repositories
{
public class AddressRepository : Bookstore.Domain.Addresses.IAddressRepository
    {
        private readonly ApplicationDbContext dbContext;

        public AddressRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task DeleteAsync(string sub, int id)
        {
            var address = await dbContext.Address.SingleOrDefaultAsync(x => x.Customer.Sub == sub && x.Id == id);

            if (address == null) return;

            address.IsActive = false;
        }

        public async Task<Address> GetAsync(string sub, int id)
        {
            return await dbContext.Address.SingleOrDefaultAsync(x => x.Customer.Sub == sub && x.Id == id && x.IsActive == true);
        }

        public async Task<IEnumerable<Address>> ListAsync(string sub)
        {
            return await dbContext.Address.Where(x => x.Customer.Sub == sub && x.IsActive == true).ToListAsync();
        }

        public async Task AddAsync(Address address)
        {
            await Task.Run(() => dbContext.Address.Add(address));
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
