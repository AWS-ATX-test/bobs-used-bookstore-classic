using System.Threading.Tasks;
using Bookstore.Web.Helpers;
using Bookstore.Web.ViewModel.ShoppingCart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;




namespace Bookstore.Web.Controllers
{
    // Local interface definitions to replace the missing namespace
    public interface ICustomerService
    {
    }

    public interface IShoppingCartService
    {
        Task<ShoppingCart> GetShoppingCartAsync(string correlationId);
        Task DeleteShoppingCartItemAsync(DeleteShoppingCartItemDto dto);
    }

    public class ShoppingCart
    {
        // Minimal implementation to support the controller
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
    public class ShoppingCartController : Controller
    {
        private readonly ICustomerService customerService;
        private readonly IShoppingCartService shoppingCartService;

        public ShoppingCartController(ICustomerService customerService, IShoppingCartService shoppingCartService)
        {
            this.customerService = customerService;
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<ActionResult> Index()
        {
            var shoppingCart = await shoppingCartService.GetShoppingCartAsync(HttpContext.GetShoppingCartCorrelationId());

            return View(new ShoppingCartIndexViewModel(shoppingCart));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int shoppingCartItemId)
        {
            var dto = new DeleteShoppingCartItemDto(HttpContext.GetShoppingCartCorrelationId(), shoppingCartItemId);

            await shoppingCartService.DeleteShoppingCartItemAsync(dto);

            this.SetNotification("Item removed from shopping cart.");

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}