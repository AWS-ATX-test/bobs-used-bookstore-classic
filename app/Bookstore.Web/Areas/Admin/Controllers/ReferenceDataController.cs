using Bookstore.Domain.ReferenceData;
using Bookstore.Web.Areas.Admin.Models.ReferenceData;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace Bookstore.Web.Areas.Admin.Controllers
{
    public class ReferenceDataController : AdminAreaControllerBase
    {
        private readonly IReferenceDataService referenceDataService;

        public ReferenceDataController(IReferenceDataService referenceDataService)
        {
            this.referenceDataService = referenceDataService;
        }

        public async Task<ActionResult> Index(object filters, int pageIndex = 1, int pageSize = 10)
        {
            var referenceDataItems = await referenceDataService.GetReferenceDataAsync(filters, pageIndex, pageSize);

            return View(new ReferenceDataIndexViewModel(referenceDataItems, filters as dynamic));
        }

        public ActionResult Create(ReferenceDataType? selectedReferenceDataType = null)
        {
            var model = new ReferenceDataItemCreateUpdateViewModel();

            if (selectedReferenceDataType.HasValue) model.SelectedReferenceDataType = selectedReferenceDataType.Value;

            return View("CreateUpdate", model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ReferenceDataItemCreateUpdateViewModel model)
        {
            await referenceDataService.CreateAsync(model.SelectedReferenceDataType, model.Text);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Update(int id)
        {
            var referenceDataItem = await referenceDataService.GetReferenceDataItemAsync(id);

            return View("CreateUpdate", new ReferenceDataItemCreateUpdateViewModel(referenceDataItem));
        }

        [HttpPost]
        public async Task<ActionResult> Update(ReferenceDataItemCreateUpdateViewModel model)
        {
            await referenceDataService.UpdateAsync(model.Id, model.SelectedReferenceDataType, model.Text);

            return RedirectToAction("Index");
        }
    }
}
