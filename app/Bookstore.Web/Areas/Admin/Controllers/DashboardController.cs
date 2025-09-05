using Bookstore.Domain.Books;
// using Bookstore.Domain.Orders; - Namespace doesn't exist
using Bookstore.Web.Areas.Admin.Models.Dashboard;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Web.Areas.Admin.Controllers
{
    // Local interface to replace the missing IOrderService
    public interface IOrderService
    {
        Task<OrderStatistics> GetStatisticsAsync();
    }

    // Local class to replace the missing OrderStatistics
    public class OrderStatistics
    {
        public int PastDueOrders { get; set; }
        public int PendingOrders { get; set; }
        public int OrdersThisMonth { get; set; }
        public int OrdersTotal { get; set; }
    }

    // Local interface to replace the missing Bookstore.Domain.Offers.IOfferService
    public interface IOfferService
    {
        Task<OfferStatistics> GetStatisticsAsync();
    }

    // Local class to replace the missing Bookstore.Domain.Offers.OfferStatistics
    public class OfferStatistics
    {
        public int PendingOffers { get; set; }
        public int OffersThisMonth { get; set; }
        public int OffersTotal { get; set; }
    }

    public class DashboardController : AdminAreaControllerBase
    {
        private readonly IOrderService orderService;
        private readonly IOfferService offerService;
        private readonly IBookService bookService;

        public DashboardController(IOrderService orderService, IOfferService offerService, IBookService bookService)
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