using Bookstore.Domain.ReferenceData;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace Bookstore.Web.Areas.Admin.Models.ReferenceData
{
    public class ReferenceDataItemCreateUpdateViewModel
    {
        public ReferenceDataItemCreateUpdateViewModel() { }

        public ReferenceDataItemCreateUpdateViewModel(ReferenceDataItem referenceDataItem)
        {
            Id = referenceDataItem.Id;
            SelectedReferenceDataType = referenceDataItem.DataType;
            Text = referenceDataItem.Text;
        }

        public int Id { get; set; }

        public int SelectedReferenceDataType { get; set; }  // Changed from ReferenceDataType to int

        public string Text { get; set; }

        public IEnumerable<SelectListItem> DataTypes { get; set; }
    }
}