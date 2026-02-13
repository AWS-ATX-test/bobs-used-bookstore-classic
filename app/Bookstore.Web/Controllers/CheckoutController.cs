using Bookstore.Domain.Addresses;
using Bookstore.Domain.Carts;
using Bookstore.Domain.Orders;
using Bookstore.Web.Helpers;
using Bookstore.Web.ViewModel.Checkout;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;

public interface IShoppingCartService
{
    Task<object> GetShoppingCartAsync(string correlationId);
}


namespace Bookstore.Web.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IAddressService addressService;
        private readonly IShoppingCartService shoppingCartService;
        private readonly IOrderService orderService;

        public CheckoutController(IShoppingCartService shoppingCartService,
                                  IOrderService orderService,
                                  IAddressService addressService)
        {
            this.shoppingCartService = shoppingCartService;
            this.orderService = orderService;
            this.addressService = addressService;
        }

        public async Task<ActionResult> Index()
        {
            var correlationId = HttpContext.Session.TryGetValue("ShoppingCartCorrelationId", out var id) ? Encoding.UTF8.GetString(id) : null;
            var shoppingCart = await shoppingCartService.GetShoppingCartAsync(correlationId);
            var addresses = await addressService.GetAddressesAsync(User.GetSub());

            return View(new CheckoutIndexViewModel(shoppingCart, addresses));
        }

        [HttpPost]
        public async Task<ActionResult> Index(CheckoutIndexViewModel model)
        {
            if(!ModelState.IsValid) return  View(model);

            var correlationId = HttpContext.Session.TryGetValue("ShoppingCartCorrelationId", out var id) ? Encoding.UTF8.GetString(id) : null;
            var dto = new { UserId = User.GetSub(), CorrelationId = correlationId, AddressId = model.SelectedAddressId };

            var orderId = await orderService.CreateOrderAsync(dto);

            return RedirectToAction("Finished", new { orderId });
        }

        public async Task<ActionResult> Finished(int orderId)
        {
            var order = await orderService.GetOrderAsync(orderId);

            return View(new CheckoutFinishedViewModel(order));
        }
    }
}
