using System.ComponentModel.DataAnnotations.Schema;
using Bookstore.Domain;
using Bookstore.Domain.Books;
// The Orders namespace doesn't exist, so we'll use types directly from Bookstore.Domain
// ReferenceData namespace doesn't exist either, ReferenceDataItem is directly in Bookstore.Domain
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

// Define Book class in Bookstore.Domain.Books namespace
namespace Bookstore.Domain.Books
{
    public partial class Book
    {
        public int Id { get; set; }
        public int PublisherId { get; set; }
        public int BookTypeId { get; set; }
        public int GenreId { get; set; }
        public int ConditionId { get; set; }

        // Navigation properties
        public ReferenceDataItem Publisher { get; set; }
        public ReferenceDataItem BookType { get; set; }
        public ReferenceDataItem Genre { get; set; }
        public ReferenceDataItem Condition { get; set; }
        // Add other properties as needed
    }
}

// Define Customer class in Bookstore.Domain namespace
namespace Bookstore.Domain
{
    public class Customer
    {
        public int Id { get; set; }
        public string Sub { get; set; }
        // Add other properties as needed
    }

    public class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }
        // Add other properties as needed
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        // Add other properties as needed
    }

    public class ShoppingCart
    {
        public int Id { get; set; }
        // Add other properties as needed
    }

    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public int ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        // Add other properties as needed
    }

    public partial class Offer
    {
        public int Id { get; set; }
        public int PublisherId { get; set; }
        public int BookTypeId { get; set; }
        public int GenreId { get; set; }
        public int ConditionId { get; set; }

        // Navigation properties
        public ReferenceDataItem Publisher { get; set; }
        public ReferenceDataItem BookType { get; set; }
        public ReferenceDataItem Genre { get; set; }
        public ReferenceDataItem Condition { get; set; }
    }

    public class ReferenceDataItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        // Add other properties as needed
    }
}

namespace Bookstore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(string connectionString) : base(connectionString) { }

        // TODO: Implement or reference the Address class properly
        // public DbSet<Address> Address { get; set; }

        public DbSet<Book> Book { get; set; }

        public DbSet<Customer> Customer { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<ShoppingCart> ShoppingCart { get; set; }

        public DbSet<OrderItem> OrderItem { get; set; }

        public DbSet<Offer> Offer { get; set; }

        public DbSet<ReferenceDataItem> ReferenceData { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Update to remove the pluralization to match the modern version
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();


            modelBuilder.Entity<Customer>().Property(x => x.Sub).HasColumnType("nvarchar").HasMaxLength(450);
            modelBuilder.Entity<Customer>().HasIndex(x => x.Sub).IsUnique();

            modelBuilder.Entity<Book>().HasRequired(x => x.Publisher).WithMany().HasForeignKey(x => x.PublisherId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Book>().HasRequired(x => x.BookType).WithMany().HasForeignKey(x => x.BookTypeId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Book>().HasRequired(x => x.Genre).WithMany().HasForeignKey(x => x.GenreId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Book>().HasRequired(x => x.Condition).WithMany().HasForeignKey(x => x.ConditionId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Offer>().HasRequired(x => x.Publisher).WithMany().HasForeignKey(x => x.PublisherId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Offer>().HasRequired(x => x.BookType).WithMany().HasForeignKey(x => x.BookTypeId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Offer>().HasRequired(x => x.Genre).WithMany().HasForeignKey(x => x.GenreId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Offer>().HasRequired(x => x.Condition).WithMany().HasForeignKey(x => x.ConditionId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>().HasRequired(x => x.Customer).WithMany().WillCascadeOnDelete(false);

            // Update the Refernce Data Table to Match the modern version
            modelBuilder.Entity<ReferenceDataItem>().ToTable("ReferenceData");

            modelBuilder.Entity<ShoppingCartItem>().HasKey(x => new { x.Id, x.ShoppingCartId });
            modelBuilder.Entity<ShoppingCartItem>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Database.SetInitializer(new BookstoreDbInitializer());
        }
    }
}