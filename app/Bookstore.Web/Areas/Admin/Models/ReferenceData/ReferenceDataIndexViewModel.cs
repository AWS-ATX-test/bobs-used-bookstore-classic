using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.Areas.Admin.Models.ReferenceData
{
    public class ReferenceDataIndexViewModel : PaginatedViewModel
    {
        public List<ReferenceDataIndexListItemViewModel> Items { get; set; } = new List<ReferenceDataIndexListItemViewModel>();

        public object Filters { get; set; }

        public ReferenceDataIndexViewModel(List<object> referenceDataItems, object filters)
        {
            // Placeholder implementation – actual logic depends on real data shape foreach (var item in referenceDataItems)
            {
                Items.Add(new ReferenceDataIndexListItemViewModel
                {
                    Id = 0,
                    Text = "Placeholder",
                    ReferenceDataType = "Placeholder"
                });
            }

            Filters = filters;

            PageIndex = 0;
            PageSize = referenceDataItems.Count;
            PageCount = 1;
            HasNextPage = false;
            HasPreviousPage = false;
            PaginationButtons = Enumerable.Range(0, 1).ToList();
        }
    }

    public class ReferenceDataIndexListItemViewModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string ReferenceDataType { get; set; }
    }
}
