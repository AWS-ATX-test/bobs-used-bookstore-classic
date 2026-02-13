using System.Threading.Tasks;
using Bookstore.Web.Helpers;
using Bookstore.Domain.Customers;
using Bookstore.Domain.Carts;
using Bookstore.Web.ViewModel.Wishlist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Bookstore.Web.Controllers
{
    public class MoveWishlistItemToShoppingCartDto
    {
        public string CorrelationId { get; }
        public int ShoppingCartItemId { get; }

        public MoveWishlistItemToShoppingCartDto(string correlationId, int shoppingCartItemId)
        {
            CorrelationId = correlationId;
            ShoppingCartItemId = shoppingCartItemId;
        }
    }

    public class MoveAllWishlistItemsToShoppingCartDto
    {
        public string CorrelationId { get; }

        public MoveAllWishlistItemsToShoppingCartDto(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }

    public class DeleteShoppingCartItemDto
    {
        public string CorrelationId { get; }
        public int ShoppingCartItemId { get; }

        public DeleteShoppingCartItemDto(string correlationId, int shoppingCartItemId)
        {
            CorrelationId = correlationId;
            ShoppingCartItemId = shoppingCartItemId;
        }
    }

    [AllowAnonymous]
    public class WishlistController : Controller
    {
        private readonly ICustomerService customerService;
        private readonly IShoppingCartService shoppingCartService;

        public WishlistController(ICustomerService customerService, IShoppingCartService shoppingCartService)
        {
            this.customerService = customerService;
            this.shoppingCartService = shoppingCartService;
        }

        private string GetShoppingCartCorrelationId()
        {
            // Placeholder implementation – return a dummy correlation id.
            return "DummyCorrelationId";
        }

        public async Task<ActionResult> Index()
        {
            var shoppingCart = await shoppingCartService.GetShoppingCartAsync(GetShoppingCartCorrelationId());

            return View(new WishlistIndexViewModel(shoppingCart));
        }

        [HttpPost]
        public async Task<ActionResult> MoveToShoppingCart(int shoppingCartItemId)
        {
            var dto = new MoveWishlistItemToShoppingCartDto(GetShoppingCartCorrelationId(), shoppingCartItemId);

            await shoppingCartService.MoveWishlistItemToShoppingCartAsync(dto);

            this.SetNotification("Item moved to shopping cart");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> MoveAllItemsToShoppingCart()
        {
            var dto = new MoveAllWishlistItemsToShoppingCartDto(GetShoppingCartCorrelationId());

            await shoppingCartService.MoveAllWishlistItemsToShoppingCartAsync(dto);

            this.SetNotification("All items moved to shopping cart");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int shoppingCartItemId)
        {
            var dto = new DeleteShoppingCartItemDto(GetShoppingCartCorrelationId(), shoppingCartItemId);

            await shoppingCartService.DeleteShoppingCartItemAsync(dto);

            this.SetNotification("Item removed from wishlist");

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}
