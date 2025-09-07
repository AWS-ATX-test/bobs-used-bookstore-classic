using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;

namespace Bookstore.Domain.Carts
{
    public class ShoppingCart
    {
        public string CorrelationId { get; set; }
        public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
    }

    public class ShoppingCartItem
    {
        public Book Book { get; set; }
    }

    public class Book
    {
    }
}

namespace Bookstore.Data.Repositories
{
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
                .Include(x => x.ShoppingCartItems)
                .Include(x => x.ShoppingCartItems.Select(y => y.Book))
                .SingleOrDefaultAsync(x => x.CorrelationId == correlationId);
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
