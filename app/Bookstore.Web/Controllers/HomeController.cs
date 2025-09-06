using System.Diagnostics;
using Bookstore.Web.ViewModel;
using Bookstore.Domain.Books;
using System.Threading.Tasks;
using Bookstore.Web.ViewModel.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace Bookstore.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IBookService bookService;

        public HomeController(IBookService bookService)
        {
            this.bookService = bookService;
        }

        public async Task<ActionResult> Index()
        {
            var bookSearchResult = await bookService.GetBooksAsync(null, 0, 4);

            return View(new HomeIndexViewModel(bookSearchResult.Books));
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult Search()
        {
            return RedirectToAction("Index", "Search");
        }

        public ActionResult Cart()
        {
            return RedirectToAction("Index", "ShoppingCart");
        }

        public ActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id });
        }
    }
}