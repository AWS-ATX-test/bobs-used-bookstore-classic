using System;
using System.Threading.Tasks;
using Bookstore.Web.Helpers;
using Bookstore.Domain.Customers;
using Bookstore.Web.ViewModel.Wishlist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;



namespace Bookstore.Web.Controllers
{
[Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public class WishlistController : Controller
    {
        private readonly ICustomerService customerService;
        private readonly IShoppingCartService shoppingCartService;

        public WishlistController(ICustomerService customerService, IShoppingCartService shoppingCartService)
        {
            this.customerService = customerService;
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<ActionResult> Index()
        {
            var correlationId = GetShoppingCartCorrelationId();
            var shoppingCart = await shoppingCartService.GetShoppingCartAsync(correlationId);

            return View(new WishlistIndexViewModel(shoppingCart));
        }

        [HttpPost]
        public async Task<ActionResult> MoveToShoppingCart(int shoppingCartItemId)
        {
            var correlationId = GetShoppingCartCorrelationId();
            var dto = new MoveWishlistItemToShoppingCartDto(correlationId, shoppingCartItemId);

            await shoppingCartService.MoveWishlistItemToShoppingCartAsync(dto);

            this.SetNotification("Item moved to shopping cart");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> MoveAllItemsToShoppingCart()
        {
            var correlationId = GetShoppingCartCorrelationId();
            var dto = new MoveAllWishlistItemsToShoppingCartDto(correlationId);

            await shoppingCartService.MoveAllWishlistItemsToShoppingCartAsync(dto);

            this.SetNotification("All items moved to shopping cart");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int shoppingCartItemId)
        {
            var correlationId = GetShoppingCartCorrelationId();
            var dto = new DeleteShoppingCartItemDto(correlationId, shoppingCartItemId);

            await shoppingCartService.DeleteShoppingCartItemAsync(dto);

            this.SetNotification("Item removed from wishlist");

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Error()
        {
            return View();
        }

        private Guid GetShoppingCartCorrelationId()
        {
            // Try to get the correlation ID from session
            string correlationIdStr = HttpContext.Session.GetString("ShoppingCartCorrelationId");

            // If not found, we might need to create a new one
            if (string.IsNullOrEmpty(correlationIdStr))
            {
                return Guid.NewGuid();
            }

            // Try to parse the string as a Guid
            if (Guid.TryParse(correlationIdStr, out Guid correlationId))
            {
                return correlationId;
            }

            // If parsing fails, return a new Guid
            return Guid.NewGuid();
        }
    }
}