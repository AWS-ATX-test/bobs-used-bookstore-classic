
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Data.Repositories
{
    public class AddressModel
    {
        public int Id { get; set; }
        public string Sub { get; set; }
        public bool IsActive { get; set; }
        public Bookstore.Data.Customer Customer { get; set; }
    }

    public interface IAddressRepository
    {
        Task DeleteAsync(string sub, int id);
        Task<Address> GetAsync(string sub, int id);
        Task<IEnumerable<Address>> ListAsync(string sub);
        Task AddAsync(Address address);
        Task SaveChangesAsync();
    }

    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext dbContext;

        public AddressRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task DeleteAsync(string sub, int id)
        {
            var address = await dbContext.Address.SingleOrDefaultAsync(x => x.Id == id);

            if (address == null) return;

            // Assuming Address entity has IsActive; if not, adjust accordingly address.IsActive = false;
        }

        public async Task<Address> GetAsync(string sub, int id)
        {
            return await dbContext.Address.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Address>> ListAsync(string sub)
        {
            return await dbContext.Address.ToListAsync();
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
