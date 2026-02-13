
using Bookstore.Web.Areas.Admin.Models.Dashboard;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bookstore.Web.Areas.Admin.Models.Dashboard;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace Bookstore.Web.Areas.Admin.Controllers
{
    public class DashboardController : AdminAreaControllerBase
    {
        private readonly dynamic orderService;
        private readonly dynamic offerService;
        private readonly dynamic bookService;

        public DashboardController(dynamic orderService, dynamic offerService, dynamic bookService)
        {
            this.orderService = orderService;
            this.offerService = offerService;
            this.bookService = bookService;
        }

        public async Task<ActionResult> Index()
        {
            var orderStats = await orderService.GetStatisticsAsync();
            var offerStats = await offerService.GetStatisticsAsync();
            var inventoryStats = await bookService.GetStatisticsAsync();

            var model = new DashboardIndexViewModel
            {
                PastDueOrders = orderStats.PastDueOrders,
                PendingOrders = orderStats.PendingOrders,
                OrdersThisMonth = orderStats.OrdersThisMonth,
                OrdersTotal = orderStats.OrdersTotal,

                PendingOffers = offerStats.PendingOffers,
                OffersThisMonth = offerStats.OffersThisMonth,
                OffersTotal = offerStats.OffersTotal,

                LowStock = inventoryStats.LowStock,
                OutOfStock = inventoryStats.OutOfStock,
                StockTotal = inventoryStats.StockTotal
            };

            return View(model);
        }
    }
}
