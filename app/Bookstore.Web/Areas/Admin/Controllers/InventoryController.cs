using System.Threading.Tasks;
using Bookstore.Web.Areas.Admin.Models.Inventory;
using Bookstore.Domain.Books;
using Bookstore.Domain.ReferenceData;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;

// Local definitions to satisfy compilation
public class BookResult
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
}

public class CreateBookDto
{
    public required string Name { get; set; }
    public required string Author { get; set; }
    public int SelectedBookTypeId { get; set; }
    public int SelectedConditionId { get; set; }
    public int? SelectedGenreId { get; set; }
    public int? SelectedPublisherId { get; set; }
    public int? Year { get; set; }
    public string? ISBN { get; set; }
    public string? Summary { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public Stream? CoverImageStream { get; set; }
    public string? CoverImageFileName { get; set; }
}

public class UpdateBookDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Author { get; set; }
    public int SelectedBookTypeId { get; set; }
    public int SelectedConditionId { get; set; }
    public int? SelectedGenreId { get; set; }
    public int? SelectedPublisherId { get; set; }
    public int? Year { get; set; }
    public string? ISBN { get; set; }
    public string? Summary { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public Stream? CoverImageStream { get; set; }
    public string? CoverImageFileName { get; set; }
}

public class Book
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Author { get; set; } = default!;
}

public interface IBookService
{
    Task<BookResult> GetBooksAsync(object filters, int pageIndex, int pageSize);
    Task<Book> GetBookAsync(int id);
    Task<BookResult> AddAsync(CreateBookDto dto);
    Task<BookResult> UpdateAsync(UpdateBookDto dto);
}

public interface IReferenceDataService
{
    Task<IEnumerable<ReferenceDataItemDto>> GetAllReferenceDataAsync();
}

public class ReferenceDataItemDto
{
    public string Name { get; set; }
    public string Value { get; set; }
}


namespace Bookstore.Web.Areas.Admin.Controllers
{
    public class BookResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }

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
            var books = await bookService.GetBooksAsync(filters, pageIndex, pageSize);
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

        private async Task<ActionResult> ProcessBookResultAsync(InventoryCreateUpdateViewModel model, BookResult result, string successMessage)
        {
            if (result.IsSuccess)
            {
                TempData["Message"] = successMessage;

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(nameof(model.CoverImage), result.ErrorMessage);

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
