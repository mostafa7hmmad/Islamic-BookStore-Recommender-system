using CleanArchitecture.DataAccess.Contexts;
using CleanArchitecture.DataAccess.Models;
using CleanArchitecture.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Services.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetBooksByIdsAsync(List<long> ids)
        {
            return await _context.Books.Where(b => ids.Contains(b.Id)).ToListAsync();
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }
    }
}