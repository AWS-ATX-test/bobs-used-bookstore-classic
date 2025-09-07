using System;
using System.Linq;
using System.Threading.Tasks;
using Bookstore.Web.Helpers;
using Bookstore.Domain.Customers;
using Bookstore.Domain.Carts;
using Bookstore.Web.ViewModel.ShoppingCart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;


namespace Bookstore.Web.Controllers
{
[Microsoft.AspNetCore.Authorization.AllowAnonymous]
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
            string correlationId = GetCorrelationIdFromCookieOrSession();
            var shoppingCart = await shoppingCartService.GetShoppingCartAsync(correlationId);

            return View(new ShoppingCartIndexViewModel(shoppingCart));
        }

        private string GetCorrelationIdFromCookieOrSession()
        {
            // Check cookies first
            if (Request.Cookies.ContainsKey("ShoppingCartCorrelationId"))
            {
                return Request.Cookies["ShoppingCartCorrelationId"];
            }

            // Check session if available
            if (HttpContext.Session != null && HttpContext.Session.Keys.Contains("ShoppingCartCorrelationId"))
            {
                return HttpContext.Session.GetString("ShoppingCartCorrelationId");
            }

            // Generate new correlation ID if not found
            string correlationId = System.Guid.NewGuid().ToString();

            // Store in cookie
            Response.Cookies.Append("ShoppingCartCorrelationId", correlationId, new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = System.DateTime.Now.AddDays(30)
            });

            return correlationId;
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int shoppingCartItemId)
        {
            var correlationId = GetCorrelationIdFromCookieOrSession();

            await shoppingCartService.DeleteShoppingCartItemAsync(correlationId, shoppingCartItemId);

            this.SetNotification("Item removed from shopping cart.");

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}
