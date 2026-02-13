// using Bookstore.Domain;
// using Bookstore.Domain.Offers;
using Bookstore.Domain.ReferenceData;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Bookstore.Web.Areas.Admin.Models.Offers
{
    public enum ReferenceDataType
    {
        Genre,
        Condition
    }
    public class OfferIndexViewModel : PaginatedViewModel
    {
public OfferIndexViewModel(IEnumerable<ReferenceDataItem> offers, IEnumerable<ReferenceDataItem> referenceData)
        {
            foreach (var offer in offers)
            {
                Items.Add(new OfferIndexItemViewModel
                {
                    OfferId = offer.Id,
                    BookName = offer.Text,
                    Author = string.Empty,
                    Genre = string.Empty,
                    CustomerName = string.Empty,
                    OfferStatus = 0,
                    OfferDate = DateTime.Now,
                    OfferPrice = 0m,
                    Condition = string.Empty
                });
            }



            Genres = referenceData.Where(x => x.DataType == ReferenceDataType.Genre).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text });
            BookConditions = referenceData.Where(x => x.DataType == ReferenceDataType.Condition).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text });
        }

        public List<OfferIndexItemViewModel> Items { get; set; } = new List<OfferIndexItemViewModel>();

        public OfferFilters Filters { get; set; }

        public IEnumerable<SelectListItem> Genres { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> BookConditions { get; set; } = new List<SelectListItem>();
    }

    public class OfferIndexItemViewModel
    {
        public int OfferId { get; set; }

        public string BookName { get; set; }

        public string CustomerName { get; set; }

        public string Author { get; set; }

        public string Genre { get; set; }

        public OfferStatus OfferStatus { get; set; }

        public DateTime OfferDate { get; internal set; }

        public decimal OfferPrice { get; internal set; }

        public string Condition { get; internal set; }
    }

    public class OfferFilters
    {
        public string BookName { get; set; }
        public string Author { get; set; }
        public int? GenreId { get; set; }
        public int? ConditionId { get; set; }
        public int? OfferStatusId { get; set; }
        public string CustomerName { get; set; }
    }

    public enum OfferStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2,
        Expired = 3
    }
}
