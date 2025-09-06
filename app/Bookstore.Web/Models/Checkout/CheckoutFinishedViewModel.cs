using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.ViewModel.Checkout
{
    public class CheckoutFinishedViewModel
    {
        public IEnumerable<CheckoutFinishedItemViewModel> Items { get; set; } = new List<CheckoutFinishedItemViewModel>();

        // Default constructor
        public CheckoutFinishedViewModel() { }

        // Constructor that accepts order items directly
        public CheckoutFinishedViewModel(IEnumerable<OrderItemData> orderItems)
        {
            if (orderItems != null)
            {
                Items = orderItems.Select(x => new CheckoutFinishedItemViewModel
                {
                    BookId = x.BookId,
                    Bookname = x.BookName,
                    Price = x.BookPrice,
                    Quantity = x.Quantity,
                    Url = x.BookCoverImageUrl
                });
            }
        }
    }

    // Simple data structure to hold order item information
    public class OrderItemData
    {
        public long BookId { get; set; }
        public string BookName { get; set; }
        public decimal BookPrice { get; set; }
        public int Quantity { get; set; }
        public string BookCoverImageUrl { get; set; }
    }

    public class CheckoutFinishedItemViewModel
    {
        public string Bookname { get; set; }

        public long BookId { get; set; }

        public int Quantity { get; set; }

        public string Url { get; set; }

        public decimal Price { get; set; }
    }
}