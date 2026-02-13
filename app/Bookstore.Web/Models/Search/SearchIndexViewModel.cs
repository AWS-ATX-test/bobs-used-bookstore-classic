using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Bookstore.Domain.Books;
using Bookstore.Domain;
using System.Linq;
using System;

namespace Bookstore.Web.ViewModel.Search
{
    public interface IPaginatedList<T> : IEnumerable<T>
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        IEnumerable<int> GetPageList(int pageListSize);
    }

    public class PaginatedList<T> : List<T>, IPaginatedList<T>
    {
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages { get; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PaginatedList(IEnumerable<T> source, int count, int pageIndex, int pageSize)
        {
            AddRange(source);
            TotalCount = count;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public IEnumerable<int> GetPageList(int pageListSize)
        {
            var current = PageIndex;
            var start = Math.Max(1, current - pageListSize / 2);
            var end = Math.Min(TotalPages, current + pageListSize / 2);
            return Enumerable.Range(start, end - start + 1);
        }
    }

    public class SearchIndexViewModel : PaginatedViewModel
    {
        public string SearchString { get; set; }

        public string SortBy { get; set; }

        public List<SearchIndexItemViewModel> Books { get; set; } = new List<SearchIndexItemViewModel>();

        public SearchIndexViewModel(IPaginatedList<SearchIndexItemViewModel> books)
        {
            foreach (var book in books)
            {
                Books.Add(new SearchIndexItemViewModel
                {
                    BookId = book.BookId,
                    BookName = book.BookName,
                    ImageUrl = book.ImageUrl,
                    Price = book.Price,
                    Quantity = book.Quantity
                });
            }

            PageIndex = books.PageIndex;
            PageSize = books.TotalCount;
            PageCount = books.TotalPages;
            HasNextPage = books.HasNextPage;
            HasPreviousPage = books.HasPreviousPage;
            PaginationButtons = books.GetPageList(5).ToList();
        }
    }

    public class SearchIndexItemViewModel
    {
        public int BookId { get; set; }

        [Display(Name = "Title")]
        [DefaultValue("Title")]
        public string BookName { get; set; }

        [DefaultValue("Publisher not found")]
        public string PublisherName { get; set; }

        [DefaultValue("No Author")]
        public string Author { get; set; }

        [Display(Name = "Genre")]
        public string GenreName { get; set; }

        [Display(Name = "Type")]
        public string TypeName { get; set; }

        [Display(Name = "Condition")]
        public string ConditionName { get; set; }

        public string ImageUrl { get; set; }

        [Display(Name = "$$")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
