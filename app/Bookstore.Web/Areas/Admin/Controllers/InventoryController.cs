using System.Threading.Tasks;
using Bookstore.Web.Areas.Admin.Models.Inventory;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;
using Bookstore.Domain.Books;
using Bookstore.Domain.ReferenceData;

// Temporary namespace and classes to fix compilation error
namespace Bookstore.Domain.Books
{
    public interface IBookService
    {
        Task<BookSearchResult> GetBooksAsync(BookFilters filters, int pageIndex, int pageSize);
        Task<Book> GetBookAsync(int id);
        Task<BookResult> AddAsync(CreateBookDto dto);
        Task<BookResult> UpdateAsync(UpdateBookDto dto);
    }

    public class BookResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public int BookTypeId { get; set; }
        public int ConditionId { get; set; }
        public int GenreId { get; set; }
        public int PublisherId { get; set; }
        public int Year { get; set; }
        public string ISBN { get; set; }
        public string Summary { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string CoverImageUrl { get; set; }
    }

    public class BookSearchResult
    {
        public IEnumerable<Book> Books { get; set; }
        public int TotalCount { get; set; }
    }

    public class BookFilters
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public int? BookTypeId { get; set; }
        public int? ConditionId { get; set; }
        public int? GenreId { get; set; }
        public int? PublisherId { get; set; }
    }

    public class CreateBookDto
    {
        public CreateBookDto(string name, string author, int bookTypeId, int conditionId, int genreId, int publisherId,
            int year, string isbn, string summary, decimal price, int quantity, Stream coverImageStream, string coverImageFileName)
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
            CoverImageFileName = coverImageFileName;
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
        public string CoverImageFileName { get; }
    }

    public class UpdateBookDto
    {
        public UpdateBookDto(int id, string name, string author, int bookTypeId, int conditionId, int genreId, int publisherId,
            int year, string isbn, string summary, decimal price, int quantity, Stream coverImageStream, string coverImageFileName)
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
            CoverImageFileName = coverImageFileName;
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
        public string CoverImageFileName { get; }
    }
}

namespace Bookstore.Domain.ReferenceData
{
    public interface IReferenceDataService
    {
        Task<IEnumerable<ReferenceDataItem>> GetAllReferenceDataAsync();
    }

    public class ReferenceDataItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
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

        public async Task<ActionResult> Index(BookFilters filters, int pageIndex = 1, int pageSize = 10)
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