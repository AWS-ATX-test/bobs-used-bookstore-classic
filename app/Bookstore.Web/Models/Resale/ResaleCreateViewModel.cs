
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Bookstore.Web.ViewModel.Resale
{
    public class ResaleCreateViewModel
    {
        public ResaleCreateViewModel() { }

        public ResaleCreateViewModel(IEnumerable<string> referenceDataItems)
        {
            BookTypes = referenceDataItems.Where(x => x.StartsWith("BookType:")).Select(x => new SelectListItem { Value = x.Split(':')[0], Text = x.Split(':')[1] });
            Publishers = referenceDataItems.Where(x => x.StartsWith("Publisher:")).Select(x => new SelectListItem { Value = x.Split(':')[0], Text = x.Split(':')[1] });
            Genres = referenceDataItems.Where(x => x.StartsWith("Genre:")).Select(x => new SelectListItem { Value = x.Split(':')[0], Text = x.Split(':')[1] });
            Conditions = referenceDataItems.Where(x => x.StartsWith("Condition:")).Select(x => new SelectListItem { Value = x.Split(':')[0], Text = x.Split(':')[1] });
        }

        public IEnumerable<SelectListItem> BookTypes { get; internal set; }

        public IEnumerable<SelectListItem> Publishers { get; internal set; }

        public IEnumerable<SelectListItem> Genres { get; internal set; }

        public IEnumerable<SelectListItem> Conditions { get; internal set; }

        public int SelectedBookTypeId { get; set; }

        public int SelectedPublisherId { get; set; }

        public int SelectedGenreId { get; set; }

        public int SelectedConditionId { get; set; }

        public decimal BookPrice { get; set; }

        public string BookName { get; set; }

        public string Author { get; set; }

        public string ISBN { get; set; }
    }
}
