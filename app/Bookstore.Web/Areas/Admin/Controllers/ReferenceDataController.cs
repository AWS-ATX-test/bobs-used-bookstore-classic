using Bookstore.Domain.ReferenceData;
using Bookstore.Web.Areas.Admin.Models.ReferenceData;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Domain.ReferenceData
{
    public enum ReferenceDataType
    {
        // Add enum values as needed based on how it's used
        // These are placeholder values
        None = 0,
        Category = 1,
        Publisher = 2,
        Author = 3
    }

    public class ReferenceDataFilters
    {
        // Basic implementation of the filters class
        // Add properties as needed based on how it's used in the service
    }
}

namespace Bookstore.Web.Areas.Admin.Models.ReferenceData
{
    public interface IPaginatedList<T>
    {
        IEnumerable<T> Items { get; }
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }

    public class PaginatedList<T> : IPaginatedList<T>
    {
        public IEnumerable<T> Items { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PaginatedList(IEnumerable<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }

        public static IPaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}

// Extension interface to add missing methods
namespace Bookstore.Web.Areas.Admin.Controllers
{
    public static class ReferenceDataServiceExtensions
    {
        public static Task<IEnumerable<ReferenceDataItem>> GetReferenceDataAsync(
            this IReferenceDataService service,
            ReferenceDataFilters filters,
            int pageIndex,
            int pageSize)
        {
            // This is a temporary implementation until the actual service is updated
            // In a real scenario, this would call the appropriate method on the service
            return Task.FromResult<IEnumerable<ReferenceDataItem>>(new List<ReferenceDataItem>());
        }
    }
}



namespace Bookstore.Web.Areas.Admin.Controllers
{
    public class ReferenceDataController : AdminAreaControllerBase
    {
        private readonly IReferenceDataService referenceDataService;

        public ReferenceDataController(IReferenceDataService referenceDataService)
        {
            this.referenceDataService = referenceDataService;
        }

        public async Task<ActionResult> Index(ReferenceDataFilters filters, int pageIndex = 1, int pageSize = 10)
        {
            var referenceDataItems = await referenceDataService.GetReferenceDataAsync(filters, pageIndex, pageSize);
            var paginatedList = Bookstore.Web.Areas.Admin.Models.ReferenceData.PaginatedList<ReferenceDataItem>.Create(referenceDataItems, pageIndex, pageSize);

            return View(new ReferenceDataIndexViewModel(paginatedList, filters));
        }

        public ActionResult Create(ReferenceDataType? selectedReferenceDataType = null)
        {
            var model = new ReferenceDataItemCreateUpdateViewModel();

            if (selectedReferenceDataType.HasValue) model.SelectedReferenceDataType = (int)selectedReferenceDataType.Value;

            return View("CreateUpdate", model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ReferenceDataItemCreateUpdateViewModel model)
        {
            var dto = new CreateReferenceDataItemDto(model.SelectedReferenceDataType, model.Text);

            await referenceDataService.CreateAsync(dto);

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
            var dto = new UpdateReferenceDataItemDto(model.Id, model.SelectedReferenceDataType, model.Text);

            await referenceDataService.UpdateAsync(dto);

            return RedirectToAction("Index");
        }
    }
}