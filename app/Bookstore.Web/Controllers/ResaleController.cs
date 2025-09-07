using System.Threading.Tasks;
using Bookstore.Web.ViewModel.Resale;
using Bookstore.Web.Helpers;
using Bookstore.Domain.Offers;
using Bookstore.Domain.ReferenceData;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Domain.Offers
{
    public class CreateOfferDto
    {
        public string UserId { get; }
        public string BookName { get; }
        public string Author { get; }
        public string ISBN { get; }
        public int BookTypeId { get; }
        public int ConditionId { get; }
        public int GenreId { get; }
        public int PublisherId { get; }
        public decimal BookPrice { get; }

        public CreateOfferDto(
            string userId,
            string bookName,
            string author,
            string isbn,
            int bookTypeId,
            int conditionId,
            int genreId,
            int publisherId,
            decimal bookPrice)
        {
            UserId = userId;
            BookName = bookName;
            Author = author;
            ISBN = isbn;
            BookTypeId = bookTypeId;
            ConditionId = conditionId;
            GenreId = genreId;
            PublisherId = publisherId;
            BookPrice = bookPrice;
        }
    }
}


namespace Bookstore.Web.Controllers
{
    public class ResaleController : Controller
    {
        private readonly IReferenceDataService referenceDataService;
        private readonly IOfferService offerService;

        public ResaleController(IReferenceDataService referenceDataService, IOfferService offerService)
        {
            this.referenceDataService = referenceDataService;
            this.offerService = offerService;
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

            var dto = new CreateOfferDto(
                User.GetSub(),
                resaleViewModel.BookName,
                resaleViewModel.Author,
                resaleViewModel.ISBN,
                resaleViewModel.SelectedBookTypeId,
                resaleViewModel.SelectedConditionId,
                resaleViewModel.SelectedGenreId,
                resaleViewModel.SelectedPublisherId,
                resaleViewModel.BookPrice);

            await offerService.CreateOfferAsync(dto);

            return RedirectToAction(nameof(Index));
        }
    }
}
