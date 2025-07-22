namespace CleanArchitecture.Services.Interfaces
{
    public interface IDataSeedingService
    {
        Task<string> SeedBookCategoriesAsync();
        Task<string> SeedBooksAsync();
        Task<string> SeedUsersAndProfilesAsync(string csvFilePath);
        Task<(string CategoriesReport, string BooksReport, string UsersReport)> SeedAllAsync(string csvFilePath);
    }

}
