using CleanArchitecture.DataAccess.Contexts;
using CleanArchitecture.DataAccess.Models;
using CleanArchitecture.Services.Interfaces;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Globalization;

namespace CleanArchitecture.Services.Services
{
    public class DataSeedingService : IDataSeedingService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public DataSeedingService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        public async Task<(string CategoriesReport, string BooksReport, string UsersReport)> SeedAllAsync(string csvFilePath)
        {
            if (!_env.IsDevelopment())
            {
                throw new InvalidOperationException("Only allowed in development environment.");
            }

            var cat = await SeedBookCategoriesAsync();
            var books = await SeedBooksAsync();
            var users = await SeedUsersAndProfilesAsync(csvFilePath);

            return (cat, books, users);
        }

        public async Task<string> SeedBookCategoriesAsync()
        {
            if (await _context.BookCategories.AnyAsync())
                return "Book categories already exist in the database.";

            var categories = new List<BookCategory>
        {
            new BookCategory { Id = 0, Name = "Aqeedah (Creed)" },
            new BookCategory { Id = 1, Name = "Fiqh (Jurisprudence)" },
            new BookCategory { Id = 2, Name = "Hadith" },
            new BookCategory { Id = 3, Name = "Seerah (Prophetic Biography)" },
            new BookCategory { Id = 4, Name = "Tafsir & General" }
        };

            await _context.BookCategories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
            return $"{categories.Count} categories seeded.";
        }

        public async Task<string> SeedBooksAsync()
        {
            if (await _context.Books.AnyAsync())
                return "Books already exist in the database.";

            var books = new List<Book>
        {
            new Book { Id = 0, Title = "Al-Wajiz in Fiqh", BookCategoryId = 1, AuthorId = 1, TopicId = 2 },
            // باقي الكتب كما في كودك الأصلي...
        };

            await _context.Books.AddRangeAsync(books);
            await _context.SaveChangesAsync();
            return $"{books.Count} books seeded.";
        }

        public async Task<string> SeedUsersAndProfilesAsync(string csvFilePath)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, csvFilePath);
            if (!File.Exists(fullPath))
                return $"CSV file not found at {fullPath}";

            using var reader = new StreamReader(fullPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<UserDataRecord>()
                             .GroupBy(r => r.user_idx)
                             .Select(g => g.First())
                             .ToList();

            int created = 0, updated = 0;

            foreach (var record in records)
            {
                var email = $"user_{record.user_idx}@gmail.com";
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = "User",
                        LastName = record.user_idx.ToString(),
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, "DefaultPassword123!");
                    if (!result.Succeeded)
                        continue;

                    created++;
                }
                else
                {
                    updated++;
                }

                user.Age = (int)(record.age * 100);
                user.Gender = MapGender(record.gender);
                user.Location = MapCountry(record.country);
                user.IsNewMuslim = record.is_new_muslim == 1 ? "Yes" : "No";
                user.BornMuslim = record.born_muslim == 1 ? "Yes" : "No";
                user.EducationLevel = MapEducation(record.education_level);
                user.ReligiousLevel = MapReligiousLevel(record.religious_level);
                user.PreferredTopic = MapTopic(record.topic_idx);

                await _userManager.UpdateAsync(user);
            }

            return $"{created} created, {updated} updated.";
        }

        // helper methods
        private string MapGender(int code) => code == 1 ? "Female" : "Male";
        private string MapCountry(int code) => $"Country_Code_{code}";
        private string MapEducation(int code) => $"Education_Level_{code}";
        private string MapReligiousLevel(int code) => $"Religious_Level_{code}";
        private string MapTopic(int code) => $"Topic_ID_{code}";

        private class UserDataRecord
        {
            public int user_idx { get; set; }
            public float age { get; set; }
            public int gender { get; set; }
            public int country { get; set; }
            public int is_new_muslim { get; set; }
            public int born_muslim { get; set; }
            public int education_level { get; set; }
            public int religious_level { get; set; }
            public int topic_idx { get; set; }
        }
    }
}
