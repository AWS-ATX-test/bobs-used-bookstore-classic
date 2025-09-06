using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bookstore.Domain.Addresses;

namespace Bookstore.Domain.Addresses
{
    public class Address
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public Customer Customer { get; set; }
    }

    public class Customer
    {
        public string Sub { get; set; }
    }

    public interface IAddressRepository
    {
        Task AddAsync(Address address);
        Task DeleteAsync(string sub, int id);
        Task<Address> GetAsync(string sub, int id);
        Task<IEnumerable<Address>> ListAsync(string sub);
        Task SaveChangesAsync();
    }
}

namespace Bookstore.Data.Repositories
{
    public class AddressRepository : Bookstore.Domain.Addresses.IAddressRepository
    {
        private readonly ApplicationDbContext dbContext;

        public AddressRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task Bookstore.Domain.Addresses.IAddressRepository.DeleteAsync(string sub, int id)
        {
            // Find addresses with the specified ID
            var address = await dbContext.Address.FirstOrDefaultAsync(x => x.Id == id);
            if (address == null) return;

            // Check if the address belongs to the specified customer
            var customerProp = address.GetType().GetProperty("Customer");
            if (customerProp != null)
            {
                var customer = customerProp.GetValue(address);
                if (customer != null)
                {
                    var subProp = customer.GetType().GetProperty("Sub");
                    if (subProp != null)
                    {
                        var addressSub = subProp.GetValue(customer)?.ToString();
                        if (addressSub != sub) return;
                    }
                }
            }

            // Access the property through reflection to handle potential mismatch in entity types
            var property = address.GetType().GetProperty("IsActive");
            if (property != null)
            {
                property.SetValue(address, false);
            }
        }

        async Task<Bookstore.Domain.Addresses.Address> Bookstore.Domain.Addresses.IAddressRepository.GetAsync(string sub, int id)
        {
            // Find addresses with the specified ID
            var address = await dbContext.Address.FirstOrDefaultAsync(x => x.Id == id);
            if (address == null) return null;

            // Check if the address is active
            var isActiveProp = address.GetType().GetProperty("IsActive");
            bool isActive = true;
            if (isActiveProp != null)
            {
                isActive = (bool)isActiveProp.GetValue(address);
            }
            if (!isActive) return null;

            // Check if the address belongs to the specified customer
            var customerProp = address.GetType().GetProperty("Customer");
            if (customerProp != null)
            {
                var customer = customerProp.GetValue(address);
                if (customer != null)
                {
                    var subProp = customer.GetType().GetProperty("Sub");
                    if (subProp != null)
                    {
                        var addressSub = subProp.GetValue(customer)?.ToString();
                        if (addressSub != sub) return null;
                    }
                }
            }

            // Convert to domain model
            return new Bookstore.Domain.Addresses.Address
            {
                Id = address.Id,
                IsActive = isActive,
                Customer = new Bookstore.Domain.Addresses.Customer { Sub = sub }
            };
        }

        async Task<IEnumerable<Bookstore.Domain.Addresses.Address>> Bookstore.Domain.Addresses.IAddressRepository.ListAsync(string sub)
        {
            var result = new List<Bookstore.Domain.Addresses.Address>();

            // Get all addresses
            var addresses = await dbContext.Address.ToListAsync();

            foreach (var address in addresses)
            {
                // Check if the address is active
                var isActiveProp = address.GetType().GetProperty("IsActive");
                bool isActive = true;
                if (isActiveProp != null)
                {
                    isActive = (bool)isActiveProp.GetValue(address);
                }
                if (!isActive) continue;

                // Check if the address belongs to the specified customer
                bool belongsToCustomer = false;
                var customerProp = address.GetType().GetProperty("Customer");
                if (customerProp != null)
                {
                    var customer = customerProp.GetValue(address);
                    if (customer != null)
                    {
                        var subProp = customer.GetType().GetProperty("Sub");
                        if (subProp != null)
                        {
                            var addressSub = subProp.GetValue(customer)?.ToString();
                            if (addressSub == sub)
                            {
                                belongsToCustomer = true;
                            }
                        }
                    }
                }

                if (belongsToCustomer)
                {
                    result.Add(new Bookstore.Domain.Addresses.Address
                    {
                        Id = address.Id,
                        IsActive = isActive,
                        Customer = new Bookstore.Domain.Addresses.Customer { Sub = sub }
                    });
                }
            }

            return result;
        }

        async Task Bookstore.Domain.Addresses.IAddressRepository.AddAsync(Bookstore.Domain.Addresses.Address address)
        {
            // Create a new entity
            var entity = dbContext.Address.Create();

            // Set properties
            entity.Id = address.Id;

            var isActiveProp = entity.GetType().GetProperty("IsActive");
            if (isActiveProp != null)
            {
                isActiveProp.SetValue(entity, address.IsActive);
            }

            // Set customer if available
            if (address.Customer != null)
            {
                var customerProp = entity.GetType().GetProperty("Customer");
                if (customerProp != null)
                {
                    var customerType = customerProp.PropertyType;
                    var customer = Activator.CreateInstance(customerType);

                    var subProp = customerType.GetProperty("Sub");
                    if (subProp != null)
                    {
                        subProp.SetValue(customer, address.Customer.Sub);
                    }

                    customerProp.SetValue(entity, customer);
                }
            }

            await Task.Run(() => dbContext.Address.Add(entity));
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}