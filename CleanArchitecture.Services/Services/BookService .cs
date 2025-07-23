using AutoMapper;
using CleanArchitecture.DataAccess.Contexts;
using CleanArchitecture.DataAccess.Models;
using CleanArchitecture.Services.DTOs.Book;
using CleanArchitecture.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Services.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BookService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<Book>> GetBooksByIdsAsync(List<long> ids)
        {
            return await _context.Books.Where(b => ids.Contains(b.Id)).ToListAsync();
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<IEnumerable<BookReadDTO>> GetAllBookAsync()
        {
            var books = await _context.Books.Include(b => b.BookCategory).ToListAsync();
            return _mapper.Map<IEnumerable<BookReadDTO>>(books);
        }

        public async Task<BookReadDTO?> GetBookByIdAsync(int id)
        {
            var book = await _context.Books.Include(b => b.BookCategory).FirstOrDefaultAsync(b => b.Id == id);
            return book == null ? null : _mapper.Map<BookReadDTO>(book);
        }

        public async Task<BookReadDTO> CreateBookAsync(BookCreateDTO dto)
        {
            var book = _mapper.Map<Book>(dto);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return _mapper.Map<BookReadDTO>(book);
        }

        public async Task<bool> UpdateBookAsync(int id, BookUpdateDTO dto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            _mapper.Map(dto, book);
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}