using Bookstore.Domain;
using Bookstore.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.ViewModel.Orders
{
    // Forward declaration of the Order class to resolve compilation error
    public class Order
    {
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int BookId { get; set; }
        public Book Book { get; set; }
    }

    public class Book
    {
        public string Name { get; set; }
        public string CoverImageUrl { get; set; }
        public decimal Price { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public static class OrderStatusExtensions
    {
        public static string GetDescription(this OrderStatus status)
        {
            return status.ToString();
        }
    }

    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }

        public string OrderStatus { get; set; }

        public DateTime DeliveryDate { get; set; }

        public decimal Total { get; set; }

        public List<OrderDetailsItemViewModel> OrderItems { get; set; } = new List<OrderDetailsItemViewModel>();

        public OrderDetailsViewModel(Order order)
        {
            OrderId = order.Id;
            DeliveryDate = order.DeliveryDate;
            OrderStatus = order.OrderStatus.GetDescription();
            Total = order.Total;

            OrderItems = order.OrderItems.Select(x => new OrderDetailsItemViewModel
            {
                BookId = x.BookId,
                BookName = x.Book.Name,
                ImageUrl = x.Book.CoverImageUrl,
                Price = x.Book.Price
            }).ToList();
        }
    }

    public class OrderDetailsItemViewModel
    {
        public int BookId { get; set; }

        public string ImageUrl { get; set; }

        public string BookName { get; set; }

        public decimal Price { get; set; }
    }
}