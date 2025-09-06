using Bookstore.Domain.Orders;
using System;
using System.Collections.Generic;

// Define OrderStatus enum and Order class since they're missing
namespace Bookstore.Domain.Orders
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public Address Address { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime DeliveryDate { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }

    public class Customer
    {
        public string FullName { get; set; }
    }

    public class Address
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }

    public class OrderItem
    {
        public Book Book { get; set; }
    }

    public class Book
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public BookType BookType { get; set; }
        public Condition Condition { get; set; }
        public Genre Genre { get; set; }
        public decimal Price { get; set; }
        public Publisher Publisher { get; set; }
    }

    public class BookType
    {
        public string Text { get; set; }
    }

    public class Condition
    {
        public string Text { get; set; }
    }

    public class Genre
    {
        public string Text { get; set; }
    }

    public class Publisher
    {
        public string Text { get; set; }
    }
}

namespace Bookstore.Web.Areas.Admin.Models.Orders
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }

        public OrderStatus SelectedOrderStatus { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime DeliveryDate { get; set; }

        public string CustomerName { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string Country { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Tax { get; set; }

        public decimal Total => Subtotal + Tax;

        public List<OrderDetailsItemViewModel> Items { get; set; } = new List<OrderDetailsItemViewModel>();

        public OrderDetailsViewModel() { }

        public OrderDetailsViewModel(Order order)
        {
            OrderId = order.Id;
            CustomerName = order.Customer.FullName;
            SelectedOrderStatus = order.OrderStatus;
            AddressLine1 = order.Address.AddressLine1;
            AddressLine2 = order.Address.AddressLine2;
            City = order.Address.City;
            State = order.Address.State;
            ZipCode = order.Address.ZipCode;
            Country = order.Address.Country;
            Subtotal = order.SubTotal;
            Tax = order.Tax;
            OrderDate = order.CreatedOn;
            DeliveryDate = order.DeliveryDate;

            foreach (var orderItem in order.OrderItems)
            {
                Items.Add(new OrderDetailsItemViewModel
                {
                    Author = orderItem.Book.Author,
                    BookType = orderItem.Book.BookType.Text,
                    Condition = orderItem.Book.Condition.Text,
                    Genre = orderItem.Book.Genre.Text,
                    Name = orderItem.Book.Name,
                    Price = orderItem.Book.Price,
                    Publisher = orderItem.Book.Publisher.Text
                });
            }
        }
    }

    public class OrderDetailsItemViewModel
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public string Publisher { get; set; }

        public string Genre { get; set; }

        public string BookType { get; set; }

        public string Condition { get; set; }

        public decimal Price { get; set; }
    }
}