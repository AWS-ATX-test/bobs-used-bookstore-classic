using System.Threading.Tasks;
using Bookstore.Web.Areas.Admin.Models.Orders;
using Bookstore.Domain.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Domain.Orders
{
    public class UpdateOrderStatusDto
    {
        public int OrderId { get; }
        public int OrderStatus { get; }

        public UpdateOrderStatusDto(int orderId, int orderStatus)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
        }
    }
}


namespace Bookstore.Web.Areas.Admin.Controllers
{
    public class OrdersController : AdminAreaControllerBase
    {
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public async Task<ActionResult> Index(int? orderId = null, string customerName = null, int? orderStatus = null, int pageIndex = 1, int pageSize = 10)
        {
            var filters = new { OrderId = orderId, CustomerName = customerName, OrderStatus = orderStatus };
            var orders = await orderService.GetOrdersAsync(filters, pageIndex, pageSize);

            return View(new OrderIndexViewModel(orders, filters));
        }

        public async Task<ActionResult> Details(int id)
        {
            var order = await orderService.GetOrderAsync(id);

            return View(new OrderDetailsViewModel(order));
        }

        [HttpPost]
        public async Task<ActionResult> Details(OrderDetailsViewModel model)
        {
            var dto = new UpdateOrderStatusDto(model.OrderId, model.SelectedOrderStatus);

            await orderService.UpdateOrderStatusAsync(dto);

            TempData["Message"] = "Order status has been updated";

            return RedirectToAction("Details", new { model.OrderId });
        }
    }
}
