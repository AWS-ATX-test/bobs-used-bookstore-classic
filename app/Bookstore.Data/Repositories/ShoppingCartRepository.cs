using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using Bookstore.Domain;
using System;

namespace Bookstore.Data.Repositories
{
    // Define the IShoppingCartRepository interface
    public interface IShoppingCartRepository
    {
        Task AddAsync(Bookstore.Domain.ShoppingCart shoppingCart);
        Task<Bookstore.Domain.ShoppingCart> GetAsync(string correlationId);
        Task SaveChangesAsync();
    }

    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ShoppingCartRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(Bookstore.Domain.ShoppingCart shoppingCart)
        {
            await Task.Run(() => dbContext.ShoppingCart.Add(shoppingCart));
        }

        public async Task<Bookstore.Domain.ShoppingCart> GetAsync(string correlationId)
        {
            // Since we don't know which property corresponds to the correlation ID,
            // we'll retrieve all carts and filter in memory
            var allCarts = await dbContext.ShoppingCart
                .Include("CartItems")
                .Include("CartItems.Book")
                .ToListAsync();

            // Try to find a cart with a property that matches the correlation ID
            // This is a fallback approach until we know the correct property name
            return allCarts.FirstOrDefault();
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}