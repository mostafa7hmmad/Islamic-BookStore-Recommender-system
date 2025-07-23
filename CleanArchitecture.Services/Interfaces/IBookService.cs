using CleanArchitecture.DataAccess.Models;
using CleanArchitecture.Services.DTOs.Book;

namespace CleanArchitecture.Services.Interfaces
{
    public interface IBookService
    {
        Task<List<Book>> GetBooksByIdsAsync(List<long> ids);
        Task<List<Book>> GetAllBooksAsync();

        Task<IEnumerable<BookReadDTO>> GetAllBookAsync();
        Task<BookReadDTO?> GetBookByIdAsync(int id);
        Task<BookReadDTO> CreateBookAsync(BookCreateDTO dto);
        Task<bool> UpdateBookAsync(int id, BookUpdateDTO dto);
        Task<bool> DeleteBookAsync(int id);
    }
}