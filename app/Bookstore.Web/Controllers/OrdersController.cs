using System.Threading.Tasks;
using Bookstore.Web.Helpers;
using Bookstore.Web.ViewModel.Orders;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


namespace Bookstore.Web.Controllers
{
    public interface IOrderService
    {
        Task<IEnumerable<object>> GetOrdersAsync(string userId);
        Task<object> GetOrderAsync(int id);
        Task CancelOrderAsync(object dto);
    }

    public class OrdersController : Controller
    {
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public async Task<ActionResult> Index()
        {
            var orders = await orderService.GetOrdersAsync(User.GetSub());

            return View(new OrderIndexViewModel(orders));
        }

        public async Task<ActionResult> Details(int id)
        {
            var order = await orderService.GetOrderAsync(id);

            return View(new OrderDetailsViewModel(order));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var dto = new { UserId = User.GetSub(), OrderId = id };

            await orderService.CancelOrderAsync(dto);

            return RedirectToAction("Index");
        }
    }
}
