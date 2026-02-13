using Bookstore.Domain;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;

namespace Bookstore.Data.Repositories
{
    public interface IShoppingCartRepository
    {
        Task AddAsync(ShoppingCart shoppingCart);
        Task<ShoppingCart> GetAsync(string correlationId);
        Task SaveChangesAsync();
    }

    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ShoppingCartRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(ShoppingCart shoppingCart)
        {
            await Task.Run(() => dbContext.ShoppingCart.Add(shoppingCart));
        }

        public async Task<ShoppingCart> GetAsync(string correlationId)
        {
            return await dbContext.ShoppingCart
                // .Include(x => x.ShoppingCartItems)
                // .Include(x => x.ShoppingCartItems.Select(y => y.Book))
                .SingleOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
