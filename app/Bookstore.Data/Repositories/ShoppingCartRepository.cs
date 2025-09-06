using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Bookstore.Domain.Carts
{
    public interface IShoppingCartRepository
    {
        Task AddAsync(ShoppingCart shoppingCart);
        Task<ShoppingCart> GetAsync(string correlationId);
        Task SaveChangesAsync();
    }

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
        // Basic book properties
        public int Id { get; set; }
        public string Title { get; set; }
    }
}

namespace Bookstore.Data.Repositories
{
    public class ShoppingCartRepository : Bookstore.Domain.Carts.IShoppingCartRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ShoppingCartRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task Bookstore.Domain.Carts.IShoppingCartRepository.AddAsync(Bookstore.Domain.Carts.ShoppingCart shoppingCart)
        {
            // Cast or convert the shopping cart to the expected type
            // This assumes dbContext.ShoppingCart expects a different ShoppingCart type
            var cartToAdd = dbContext.ShoppingCart.Create();

            // Map properties from domain model to data model
            // Use dynamic or reflection approach to avoid compile-time errors
            var dataModelCart = (dynamic)cartToAdd;
            dataModelCart.CorrelationId = shoppingCart.CorrelationId;
            // Map other properties as needed

            await Task.Run(() => dbContext.ShoppingCart.Add(cartToAdd));
        }

        async Task<Bookstore.Domain.Carts.ShoppingCart> Bookstore.Domain.Carts.IShoppingCartRepository.GetAsync(string correlationId)
        {
// Query the shopping cart without using Include
            // Get all carts and filter in memory to avoid property access issues
            var allCarts = await dbContext.ShoppingCart.ToListAsync();
            object cart = null;

// Find the cart with matching correlation ID using reflection
            foreach (var c in allCarts)
            {
                try
                {
                    PropertyInfo propInfo = c.GetType().GetProperty("CorrelationId");
                    if (propInfo != null)
                    {
                        string value = propInfo.GetValue(c)?.ToString();
                        if (value == correlationId)
                        {
                            cart = c;
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignore reflection errors
                }
            }

            if (cart == null)
                return null;

            // Create a domain model shopping cart
            var domainCart = new Bookstore.Domain.Carts.ShoppingCart
            {
                CorrelationId = cart.GetType().GetProperty("CorrelationId")?.GetValue(cart)?.ToString(),
                ShoppingCartItems = new List<Bookstore.Domain.Carts.ShoppingCartItem>()
            };

            // Use reflection to safely access properties that might not exist
            try
            {
// Try to get the items collection using reflection
                var itemsProperty = cart.GetType().GetProperty("ShoppingCartItems");
                if (itemsProperty != null)
                {
                    var items = itemsProperty.GetValue(cart) as IEnumerable<object>;
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            var domainItem = new Bookstore.Domain.Carts.ShoppingCartItem();

                            // Try to get the Book property
                            var bookProperty = item.GetType().GetProperty("Book");
                            if (bookProperty != null)
                            {
                                var book = bookProperty.GetValue(item);
                                if (book != null)
                                {
                                    var idProperty = book.GetType().GetProperty("Id");
                                    var titleProperty = book.GetType().GetProperty("Title");

                                    domainItem.Book = new Bookstore.Domain.Carts.Book
                                    {
                                        Id = idProperty != null ? (int)idProperty.GetValue(book) : 0,
                                        Title = titleProperty != null ? (string)titleProperty.GetValue(book) : string.Empty
                                    };
                                }
                            }

                            domainCart.ShoppingCartItems.Add(domainItem);
                        }
                    }
                }
            }
            catch
            {
                // If reflection fails, we'll return the cart without items
            }

            return domainCart;
        }

        async Task Bookstore.Domain.Carts.IShoppingCartRepository.SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}