using CleanArchitecture.DataAccess.IUnitOfWorks;
using CleanArchitecture.DataAccess.Models;
using CleanArchitecture.Services.DTOs.Book; // <-- Using your new DTO namespace
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/book
        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var bookRepository = _unitOfWork.Repository<Book>();
            // Eagerly load BookCategory to get the name for the DTO
            var books = await bookRepository.GetAllAsync(includeProperties: "BookCategory");

            // Project the database models to your BookReadDTO
            var booksDto = books.Select(book => new BookReadDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                CoverImagePath = book.CoverImagePath,
                CategoryName = book.BookCategory?.Name // Use null-conditional for safety
            });

            return Ok(booksDto);
        }

        // GET: api/book/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var bookRepository = _unitOfWork.Repository<Book>();
            var book = await bookRepository.GetAsync(b => b.Id == id, includeProperties: "BookCategory");

            if (book == null)
            {
                return NotFound();
            }

            // Project the single book to your BookReadDTO
            var bookDto = new BookReadDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                CoverImagePath = book.CoverImagePath,
                CategoryName = book.BookCategory?.Name
            };

            return Ok(bookDto);
        }

        // GET: api/book/category/Fiction
        [HttpGet("category/{categoryName}")]
        public async Task<IActionResult> GetBooksByCategory(string categoryName)
        {
            var bookRepository = _unitOfWork.Repository<Book>();

            // Filter books by the name of their related category (case-insensitive)
            var books = await bookRepository.GetAllAsync(
                b => b.BookCategory.Name.ToLower() == categoryName.ToLower(),
                includeProperties: "BookCategory"
            );

            var booksDto = books.Select(book => new BookReadDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                CoverImagePath = book.CoverImagePath,
                CategoryName = book.BookCategory?.Name
            });

            return Ok(booksDto);
        }

        // POST: api/book
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookCreateDTO bookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map the DTO to the database model
            var newBook = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                BookCategoryId = bookDto.BookCategoryId,
                CoverImagePath = bookDto.CoverImagePath
            };

            var bookRepository = _unitOfWork.Repository<Book>();
            await bookRepository.AddAsync(newBook);
            await _unitOfWork.Complete();

            // Fetch the related category to return a complete BookReadDTO
            var category = await _unitOfWork.Repository<BookCategory>().GetAsync(c => c.Id == newBook.BookCategoryId);

            // Return a 201 Created response with the created object
            var resultDto = new BookReadDTO
            {
                Id = newBook.Id,
                Title = newBook.Title,
                Author = newBook.Author,
                CoverImagePath = newBook.CoverImagePath,
                CategoryName = category?.Name
            };

            return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, resultDto);
        }

        // PUT: api/book/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookUpdateDTO bookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bookRepository = _unitOfWork.Repository<Book>();
            var existingBook = await bookRepository.GetAsync(b => b.Id == id);

            if (existingBook == null)
            {
                return NotFound();
            }

            // Map updated fields from DTO to the existing database model
            existingBook.Title = bookDto.Title;
            existingBook.Author = bookDto.Author;
            existingBook.BookCategoryId = bookDto.BookCategoryId;
            existingBook.CoverImagePath = bookDto.CoverImagePath;

            bookRepository.Update(existingBook);
            await _unitOfWork.Complete();

            return NoContent(); // Success, no content to return
        }

        // DELETE: api/book/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var bookRepository = _unitOfWork.Repository<Book>();
            var bookToDelete = await bookRepository.GetAsync(b => b.Id == id);

            if (bookToDelete == null)
            {
                return NotFound();
            }

            bookRepository.Delete(bookToDelete);
            await _unitOfWork.Complete();

            return NoContent(); // Success, no content to return
        }
    }
}