using System.Threading.Tasks;
using Bookstore.Web.Helpers;
using Bookstore.Domain.Books;
using Bookstore.Domain.Carts;
using Bookstore.Web.ViewModel.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;


namespace Bookstore.Web.Controllers
{
    public class AddToShoppingCartDto
    {
        public string CorrelationId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }

        public AddToShoppingCartDto(string correlationId, int bookId, int quantity)
        {
            CorrelationId = correlationId;
            BookId = bookId;
            Quantity = quantity;
        }
    }

    public class AddToWishlistDto
    {
        public string CorrelationId { get; set; }
        public int BookId { get; set; }

        public AddToWishlistDto(string correlationId, int bookId)
        {
            CorrelationId = correlationId;
            BookId = bookId;
        }
    }

    [AllowAnonymous]
    public class SearchController : Controller
    {
        private readonly IBookService inventoryService;
        private readonly IShoppingCartService shoppingCartService;

        public SearchController(IBookService inventoryService, IShoppingCartService shoppingCartService)
        {
            this.inventoryService = inventoryService;
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<ActionResult> Index(string searchString, string sortBy = "Name", int pageIndex = 1, int pageSize = 10)
        {
            var books = await inventoryService.GetBooksAsync(searchString, sortBy, pageIndex, pageSize);

            return View(new SearchIndexViewModel(books));
        }

        public async Task<ActionResult> Details(int id)
        {
            var book = await inventoryService.GetBookAsync(id);

            return View(new SearchDetailsViewModel(book));
        }

        public async Task<ActionResult> AddItemToShoppingCart(int bookId)
        {
            var dto = new AddToShoppingCartDto(Guid.NewGuid().ToString(), bookId, 1);

            await shoppingCartService.AddToShoppingCartAsync(dto);

            this.SetNotification("Item added to shopping cart");

            return RedirectToAction("Index", "Search");
        }

        public async Task<ActionResult> AddItemToWishlist(int bookId)
        {
            var dto = new AddToWishlistDto(Guid.NewGuid().ToString(), bookId);

            await shoppingCartService.AddToWishlistAsync(dto);

            this.SetNotification("Item added to wishlist");

            return RedirectToAction("Index", "Search");
        }
    }
}
