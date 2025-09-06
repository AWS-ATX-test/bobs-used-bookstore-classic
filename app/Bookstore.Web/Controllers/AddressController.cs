using Bookstore.Web.Helpers;
using Bookstore.Web.ViewModel.Address;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Bookstore.Domain.Addresses
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetAddressesAsync(string userId);
        Task<Address> GetAddressAsync(string userId, int addressId);
        Task CreateAddressAsync(CreateAddressDto dto);
        Task UpdateAddressAsync(UpdateAddressDto dto);
        Task DeleteAddressAsync(DeleteAddressDto dto);
    }

    public class Address
    {
        public int Id { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string UserId { get; set; }
    }

    public class CreateAddressDto
    {
        public string AddressLine1 { get; private set; }
        public string AddressLine2 { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string ZipCode { get; private set; }
        public string UserId { get; private set; }

        public CreateAddressDto(string addressLine1, string addressLine2, string city, string state, string country, string zipCode, string userId)
        {
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
            UserId = userId;
        }
    }

    public class UpdateAddressDto
    {
        public int Id { get; private set; }
        public string AddressLine1 { get; private set; }
        public string AddressLine2 { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string ZipCode { get; private set; }
        public string UserId { get; private set; }

        public UpdateAddressDto(int id, string addressLine1, string addressLine2, string city, string state, string country, string zipCode, string userId)
        {
            Id = id;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
            UserId = userId;
        }
    }

    public class DeleteAddressDto
    {
        public int Id { get; private set; }
        public string UserId { get; private set; }

        public DeleteAddressDto(int id, string userId)
        {
            Id = id;
            UserId = userId;
        }
    }
}

namespace Bookstore.Domain.Customers
{
    public interface ICustomerService
    {
        // Define minimal interface to satisfy the controller dependency
    }
}

namespace Bookstore.Web.Controllers
{
    public class AddressController : Controller
    {
        private readonly Bookstore.Domain.Addresses.IAddressService addressService;
        private readonly Bookstore.Domain.Customers.ICustomerService customerService;

        public AddressController(Bookstore.Domain.Addresses.IAddressService addressService, Bookstore.Domain.Customers.ICustomerService customerService)
        {
            this.addressService = addressService;
            this.customerService = customerService;
        }

        public async Task<ActionResult> Index()
        {
            var addresses = await addressService.GetAddressesAsync(User.GetSub());

            return View(new AddressIndexViewModel(addresses));
        }

        public ActionResult Create(string returnUrl)
        {
            var model = new AddressCreateUpdateViewModel(returnUrl);

            return View("CreateUpdate", model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(AddressCreateUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View("CreateUpdate", model);

            var dto = new Bookstore.Domain.Addresses.CreateAddressDto(model.AddressLine1, model.AddressLine2, model.City, model.State, model.Country, model.ZipCode, User.GetSub());

            await addressService.CreateAddressAsync(dto);

            return Redirect(model.ReturnUrl);
        }

        public async Task<ActionResult> Update(int id, string returnUrl)
        {
            var address = await addressService.GetAddressAsync(User.GetSub(), id);

            return View("CreateUpdate", new AddressCreateUpdateViewModel(address, returnUrl));
        }

        [HttpPost]
        public async Task<ActionResult> Update(AddressCreateUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new Bookstore.Domain.Addresses.UpdateAddressDto(model.Id, model.AddressLine1, model.AddressLine2, model.City, model.State, model.Country, model.ZipCode, User.GetSub());

            await addressService.UpdateAddressAsync(dto);

            return Redirect(model.ReturnUrl);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var dto = new Bookstore.Domain.Addresses.DeleteAddressDto(id, User.GetSub());

            await addressService.DeleteAddressAsync(dto);

            this.SetNotification("Address deleted");

            return RedirectToAction(nameof(Index));
        }
    }
}