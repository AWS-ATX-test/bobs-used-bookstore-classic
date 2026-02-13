using System.Threading.Tasks;
using Bookstore.Web.ViewModel.Resale;
using Bookstore.Web.Helpers;
using Bookstore.Domain.Offers;
using Bookstore.Domain.ReferenceData;
using Microsoft.AspNetCore.Mvc;


namespace Bookstore.Web.Controllers
{
    public class ResaleController : Controller
    {
        private readonly IReferenceDataService referenceDataService;
        private readonly object offerService = null;

        public ResaleController(IReferenceDataService referenceDataService)
        {
            this.referenceDataService = referenceDataService;
        }

        public async Task<ActionResult> Index()
        {
            var offers = await offerService.GetOffersAsync(User.GetSub());

            return View(new ResaleIndexViewModel(offers));
        }

        public async Task<ActionResult> Create()
        {
            var referenceDataDtos = await referenceDataService.GetAllReferenceDataAsync();

            return View(new ResaleCreateViewModel(referenceDataDtos));
        }

        [HttpPost]
        public async Task<ActionResult> Create(ResaleCreateViewModel resaleViewModel)
        {
            if (!ModelState.IsValid) return View();

            dynamic dto = new
            {
                UserId = User.GetSub(),
                BookName = resaleViewModel.BookName,
                Author = resaleViewModel.Author,
                ISBN = resaleViewModel.ISBN,
                BookTypeId = resaleViewModel.SelectedBookTypeId,
                ConditionId = resaleViewModel.SelectedConditionId,
                GenreId = resaleViewModel.SelectedGenreId,
                PublisherId = resaleViewModel.SelectedPublisherId,
                Price = resaleViewModel.BookPrice
            };

            await ((dynamic)offerService).CreateOfferAsync(dto);

            return RedirectToAction(nameof(Index));
        }
    }
}
