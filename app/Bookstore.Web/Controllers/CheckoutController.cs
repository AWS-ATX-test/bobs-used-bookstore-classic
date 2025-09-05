using Bookstore.Domain.Addresses;
using Bookstore.Domain.Carts;
using Bookstore.Domain.Orders;
using Bookstore.Web.Helpers;
using Bookstore.Web.ViewModel.Checkout;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Bookstore.Domain.Addresses
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetAddressesAsync(string userId);
    }

    public class Address
    {
        // Basic properties needed for the controller
        public int Id { get; set; }
    }
}

namespace Bookstore.Domain.Orders
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(CreateOrderDto dto);
        Task<Order> GetOrderAsync(int orderId);
    }

    public class CreateOrderDto
    {
        public string UserId { get; }
        public string ShoppingCartId { get; }
        public int AddressId { get; }

        public CreateOrderDto(string userId, string shoppingCartId, int addressId)
        {
            UserId = userId;
            ShoppingCartId = shoppingCartId;
            AddressId = addressId;
        }
    }

    public class Order
    {
        // Basic properties needed for the controller
        public int Id { get; set; }
    }
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
            var shoppingCart = await shoppingCartService.GetShoppingCartAsync(HttpContext.GetShoppingCartCorrelationId());
            var addresses = await addressService.GetAddressesAsync(User.GetSub());

            return View(new CheckoutIndexViewModel(shoppingCart, addresses));
        }

        [HttpPost]
        public async Task<ActionResult> Index(CheckoutIndexViewModel model)
        {
            if(!ModelState.IsValid) return  View(model);

            var dto = new CreateOrderDto(User.GetSub(), HttpContext.GetShoppingCartCorrelationId(), model.SelectedAddressId);

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