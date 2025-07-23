using CleanArchitecture.Services.DTOs.Book;
using CleanArchitecture.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IWebHostEnvironment _env;

        public BooksController(IBookService bookService, IWebHostEnvironment env)
        {
            _bookService = bookService;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] BookCreateDTO dto, IFormFile? image)
        {
            if (image != null)
            {
                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var imagePath = Path.Combine(_env.WebRootPath ?? "wwwroot", "images", imageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                dto.CoverImagePath = $"/images/{imageName}";
            }

            var newBook = await _bookService.CreateBookAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newBook.Id }, newBook);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] BookUpdateDTO dto, IFormFile? image)
        {
            if (image != null)
            {
                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var imagePath = Path.Combine(_env.WebRootPath ?? "wwwroot", "images", imageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                dto.CoverImagePath = $"/images/{imageName}";
            }

            var updated = await _bookService.UpdateBookAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _bookService.DeleteBookAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
