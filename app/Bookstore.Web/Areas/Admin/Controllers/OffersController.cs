using System.Threading.Tasks;
using Bookstore.Web.Areas.Admin.Controllers;
using Bookstore.Web.Areas.Admin.Controllers;
using Bookstore.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

public enum OfferStatus
{
    Approved,
    Rejected,
    Received,
    Paid
}

public class OfferFilters
{
}

public class OfferIndexViewModel
{
    public OfferIndexViewModel(object offers, object referenceData) { }
}

public class UpdateOfferStatusDto
{
    public UpdateOfferStatusDto(int id, OfferStatus status) { }
}


namespace Bookstore.Web.Areas.Admin.Controllers
{
    public class OffersController : AdminAreaControllerBase
    {
        private readonly object offerService = null!;
        private readonly object referenceDataService = null!;

        public OffersController(object offerService, object referenceDataService)
        {
            this.offerService = offerService;
            this.referenceDataService = referenceDataService;
        }

        public async Task<ActionResult> Index(OfferFilters filters, int pageIndex = 1, int pageSize = 10)
        {
            var offers = await offerService.GetOffersAsync(filters, pageIndex, pageSize);
            var referenceData = await referenceDataService.GetAllReferenceDataAsync();

            return View(new OfferIndexViewModel(offers, referenceData));
        }

        [HttpPost]
        public async Task<ActionResult> ApproveAsync(int id)
        {
            return await UpdateOfferStatus(id, OfferStatus.Approved, "The offer has been approved");
        }

        [HttpPost]
        public async Task<ActionResult> RejectAsync(int id)
        {
            return await UpdateOfferStatus(id, OfferStatus.Rejected, "The offer has been rejected");
        }

        [HttpPost]
        public async Task<ActionResult> ReceivedAsync(int id)
        {
            return await UpdateOfferStatus(id, OfferStatus.Received, "The book has been received");
        }

        [HttpPost]
        public async Task<ActionResult> PaidAsync(int id)
        {
            return await UpdateOfferStatus(id, OfferStatus.Paid, "The customer has been paid");
        }

        private async Task<ActionResult> UpdateOfferStatus(int id, OfferStatus status, string message)
        {
            var dto = new UpdateOfferStatusDto(id, status);

            await offerService.UpdateOfferStatusAsync(dto);

            TempData["Message"] = message;

            return RedirectToAction("Index");
        }
    }
}
