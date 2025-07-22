using CleanArchitecture.DataAccess.Models;

namespace CleanArchitecture.Services.Interfaces
{
    public interface IBookService
    {
        Task<List<Book>> GetBooksByIdsAsync(List<long> ids);
        Task<List<Book>> GetAllBooksAsync();
    }
}