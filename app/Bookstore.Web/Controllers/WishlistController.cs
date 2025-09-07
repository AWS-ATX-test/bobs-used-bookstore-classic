using System.Threading.Tasks;
using Bookstore.Web.Helpers;
using Bookstore.Domain.Customers;
using Bookstore.Domain.Carts;
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
            var shoppingCart = await shoppingCartService.GetShoppingCartAsync(HttpContext.Request.Cookies["ShoppingCartCorrelationId"]);

            return View(new WishlistIndexViewModel(shoppingCart));
        }

        [HttpPost]
        public async Task<ActionResult> MoveToShoppingCart(int shoppingCartItemId)
        {
            var correlationId = HttpContext.Request.Cookies["ShoppingCartCorrelationId"];

            await shoppingCartService.MoveWishlistItemToShoppingCartAsync(correlationId, shoppingCartItemId);

            this.SetNotification("Item moved to shopping cart");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> MoveAllItemsToShoppingCart()
        {
            var correlationId = HttpContext.Request.Cookies["ShoppingCartCorrelationId"];

            await shoppingCartService.MoveAllWishlistItemsToShoppingCartAsync(correlationId);

            this.SetNotification("All items moved to shopping cart");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int shoppingCartItemId)
        {
            var correlationId = HttpContext.Request.Cookies["ShoppingCartCorrelationId"];

            await shoppingCartService.DeleteShoppingCartItemAsync(correlationId, shoppingCartItemId);

            this.SetNotification("Item removed from wishlist");

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}
