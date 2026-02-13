using Bookstore.Web.Helpers;
using Bookstore.Web.ViewModel.Address;
using Bookstore.Domain.Addresses;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Bookstore.Domain.Addresses
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAddressesAsync(string userId);
        Task<AddressDto> GetAddressAsync(string userId, int id);
        Task CreateAddressAsync(CreateAddressDto dto);
        Task UpdateAddressAsync(UpdateAddressDto dto);
        Task DeleteAddressAsync(DeleteAddressDto dto);
    }

    public class AddressDto
    {
        public int Id { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }

    public class CreateAddressDto
    {
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
        public string AddressLine1 { get; }
        public string AddressLine2 { get; }
        public string City { get; }
        public string State { get; }
        public string Country { get; }
        public string ZipCode { get; }
        public string UserId { get; }
    }

    public class UpdateAddressDto
    {
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
        public int Id { get; }
        public string AddressLine1 { get; }
        public string AddressLine2 { get; }
        public string City { get; }
        public string State { get; }
        public string Country { get; }
        public string ZipCode { get; }
        public string UserId { get; }
    }

    public class DeleteAddressDto
    {
        public DeleteAddressDto(int id, string userId)
        {
            Id = id;
            UserId = userId;
        }
        public int Id { get; }
        public string UserId { get; }
    }
}

namespace Bookstore.Domain.Customers
{
    public interface ICustomerService
    {
    }
}


namespace Bookstore.Web.Controllers
{
    public class AddressController : Controller
    {
        private readonly IAddressService addressService;

        public AddressController(IAddressService addressService)
        {
            this.addressService = addressService;
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

            var dto = new CreateAddressDto(model.AddressLine1, model.AddressLine2, model.City, model.State, model.Country, model.ZipCode, User.GetSub());

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

            var dto = new UpdateAddressDto(model.Id, model.AddressLine1, model.AddressLine2, model.City, model.State, model.Country, model.ZipCode, User.GetSub());

            await addressService.UpdateAddressAsync(dto);

            return Redirect(model.ReturnUrl);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var dto = new DeleteAddressDto(id, User.GetSub());

            await addressService.DeleteAddressAsync(dto);

            this.SetNotification("Address deleted");

            return RedirectToAction(nameof(Index));
        }
    }
}
