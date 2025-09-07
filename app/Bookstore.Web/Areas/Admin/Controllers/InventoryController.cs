using System;
using System.IO;
using System.Threading.Tasks;
using Bookstore.Web.Areas.Admin.Models.Inventory;
using Bookstore.Domain.Books;
using Bookstore.Domain.ReferenceData;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Domain.Books
{
    public interface IBookService
    {
        Task<dynamic> GetBooksAsync(dynamic filters, int pageIndex, int pageSize);
        Task<dynamic> GetBookAsync(int id);
        Task<dynamic> AddAsync(CreateBookDto dto);
        Task<dynamic> UpdateAsync(UpdateBookDto dto);
    }

    public class CreateBookDto
    {
        public CreateBookDto(string name, string author, int bookTypeId, int conditionId, int genreId, int publisherId,
            int year, string isbn, string summary, decimal price, int quantity, Stream coverImageStream, string coverImageFilename)
        {
            Name = name;
            Author = author;
            BookTypeId = bookTypeId;
            ConditionId = conditionId;
            GenreId = genreId;
            PublisherId = publisherId;
            Year = year;
            ISBN = isbn;
            Summary = summary;
            Price = price;
            Quantity = quantity;
            CoverImageStream = coverImageStream;
            CoverImageFilename = coverImageFilename;
        }

        public string Name { get; }
        public string Author { get; }
        public int BookTypeId { get; }
        public int ConditionId { get; }
        public int GenreId { get; }
        public int PublisherId { get; }
        public int Year { get; }
        public string ISBN { get; }
        public string Summary { get; }
        public decimal Price { get; }
        public int Quantity { get; }
        public Stream CoverImageStream { get; }
        public string CoverImageFilename { get; }
    }

    public class UpdateBookDto
    {
        public UpdateBookDto(int id, string name, string author, int bookTypeId, int conditionId, int genreId, int publisherId,
            int year, string isbn, string summary, decimal price, int quantity, Stream coverImageStream, string coverImageFilename)
        {
            Id = id;
            Name = name;
            Author = author;
            BookTypeId = bookTypeId;
            ConditionId = conditionId;
            GenreId = genreId;
            PublisherId = publisherId;
            Year = year;
            ISBN = isbn;
            Summary = summary;
            Price = price;
            Quantity = quantity;
            CoverImageStream = coverImageStream;
            CoverImageFilename = coverImageFilename;
        }

        public int Id { get; }
        public string Name { get; }
        public string Author { get; }
        public int BookTypeId { get; }
        public int ConditionId { get; }
        public int GenreId { get; }
        public int PublisherId { get; }
        public int Year { get; }
        public string ISBN { get; }
        public string Summary { get; }
        public decimal Price { get; }
        public int Quantity { get; }
        public Stream CoverImageStream { get; }
        public string CoverImageFilename { get; }
    }
}

namespace Bookstore.Domain.ReferenceData
{
    public interface IReferenceDataService
    {
        Task<dynamic> GetAllReferenceDataAsync();
    }
}


namespace Bookstore.Web.Areas.Admin.Controllers
{
    public class InventoryController : AdminAreaControllerBase
    {
        private readonly IBookService bookService;
        private readonly IReferenceDataService referenceDataService;

        public InventoryController(IBookService bookService, IReferenceDataService referenceDataService)
        {
            this.bookService = bookService;
            this.referenceDataService = referenceDataService;
        }

        public async Task<ActionResult> Index(object filters, int pageIndex = 1, int pageSize = 10)
        {
            var books = await bookService.GetBooksAsync(filters as dynamic, pageIndex, pageSize);
            var referenceDataItems = await referenceDataService.GetAllReferenceDataAsync();

            return View(new InventoryIndexViewModel(books, referenceDataItems));
        }

        public async Task<ActionResult> Details(int id)
        {
            var book = await bookService.GetBookAsync(id);

            return View(new InventoryDetailsViewModel(book));
        }

        public async Task<ActionResult> Create()
        {
            var referenceDataItemDtos = await referenceDataService.GetAllReferenceDataAsync();

            return View("CreateUpdate", new InventoryCreateUpdateViewModel(referenceDataItemDtos));
        }

        [HttpPost]
        public async Task<ActionResult> Create(InventoryCreateUpdateViewModel model)
        {
            if (!ModelState.IsValid) return await InvalidCreateUpdateView(model);

            var dto = new CreateBookDto(
                model.Name,
                model.Author,
                model.SelectedBookTypeId,
                model.SelectedConditionId,
                model.SelectedGenreId,
                model.SelectedPublisherId,
                model.Year,
                model.ISBN,
                model.Summary,
                model.Price,
                model.Quantity,
                model.CoverImage?.InputStream,
                model.CoverImage?.FileName);

            var result = await bookService.AddAsync(dto);

            return await ProcessBookResultAsync(model, result, $"{model.Name} has been added to inventory");
        }

        public async Task<ActionResult> Update(int id)
        {
            var book = await bookService.GetBookAsync(id);
            var referenceDataDtos = await referenceDataService.GetAllReferenceDataAsync();

            return View("CreateUpdate", new InventoryCreateUpdateViewModel(referenceDataDtos, book));
        }

        [HttpPost]
        public async Task<ActionResult> Update(InventoryCreateUpdateViewModel model)
        {
            if (!ModelState.IsValid) return await InvalidCreateUpdateView(model);

            var dto = new UpdateBookDto(
                model.Id,
                model.Name,
                model.Author,
                model.SelectedBookTypeId,
                model.SelectedConditionId,
                model.SelectedGenreId,
                model.SelectedPublisherId,
                model.Year,
                model.ISBN,
                model.Summary,
                model.Price,
                model.Quantity,
                model.CoverImage?.InputStream,
                model.CoverImage?.FileName);

            var result = await bookService.UpdateAsync(dto);

            return await ProcessBookResultAsync(model, result, $"{model.Name} has been updated");
        }

        private async Task<ActionResult> ProcessBookResultAsync<T>(InventoryCreateUpdateViewModel model, T result, string successMessage) where T : class
        {
            // Assuming the result has IsSuccess and ErrorMessage properties through dynamic access
            dynamic dynamicResult = result;
            if (dynamicResult.IsSuccess)
            {
                TempData["Message"] = successMessage;

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(nameof(model.CoverImage), dynamicResult.ErrorMessage);

                return await InvalidCreateUpdateView(model);
            }
        }

        private async Task<ActionResult> InvalidCreateUpdateView(InventoryCreateUpdateViewModel model)
        {
            var referenceDataItemDtos = await referenceDataService.GetAllReferenceDataAsync();

            model.AddReferenceData(referenceDataItemDtos);

            return View("CreateUpdate", model);
        }
    }
}
