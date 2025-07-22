using CleanArchitecture.DataAccess.Contexts;
using CleanArchitecture.DataAccess.Models;
using CsvHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CleanArchitecture.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataSeedingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public DataSeedingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        /// <summary>
        /// Creates and seeds all necessary data: Categories, Books, and Users from a CSV file.
        /// This is a one-time operation for setting up the application.
        /// </summary>
        [HttpPost("SeedInitialData")]
        public async Task<IActionResult> SeedInitialData()
        {
            // Safety check to ensure this can only be run in a development environment.
            if (!_env.IsDevelopment())
            {
                return Forbid("This operation is only allowed in the development environment.");
            }

            // The order of these operations is critical due to database foreign key constraints.
            var categoriesReport = await SeedBookCategoriesAsync();
            var booksReport = await SeedBooksAsync();
            var userProfilesReport = await SeedUsersAndProfilesAsync("DataFiles/data.csv");

            return Ok(new
            {
                Message = "Data seeding process completed.",
                CategoriesReport = categoriesReport,
                BooksReport = booksReport,
                UserProfilesReport = userProfilesReport
            });
        }

        private async Task<string> SeedBookCategoriesAsync()
        {
            if (await _context.BookCategories.AnyAsync())
            {
                return "Book categories already exist in the database.";
            }

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

            return $"{categories.Count} categories have been successfully created.";
        }

        // In DataSeedingController.cs, replace the ENTIRE SeedBooksAsync method with this new one.

        private async Task<string> SeedBooksAsync()
        {
            if (await _context.Books.AnyAsync())
            {
                return "Books already exist in the database.";
            }

            // Seeding books with new AuthorId and TopicId fields.
            // These are best-guess placeholders.
            var books = new List<Book>
    {
        // BookCategoryId: 1:Aqeedah, 2:Fiqh, 3:Hadith, 4:Seerah, 5:Tafsir, 6:General
        // TopicId is often related to the CategoryId in simple models.
        // AuthorId is a placeholder.
           new Book { Id = 0, Title = "Al-Wajiz in Fiqh", BookCategoryId = 1, AuthorId = 1, TopicId = 2 },
                new Book { Id = 1, Title = "Aqeedah Wasitiyyah", BookCategoryId = 0, AuthorId = 2, TopicId = 1 },
                new Book { Id = 2, Title = "Aqeedah at-Tahawiyyah - Part 1", BookCategoryId = 0, AuthorId = 3, TopicId = 1 },
                new Book { Id = 3, Title = "Aqeedah at-Tahawiyyah - Part 2", BookCategoryId = 0, AuthorId = 3, TopicId = 1 },
                new Book { Id = 4, Title = "Aqeedah at-Tahawiyyah - Part 3", BookCategoryId = 0, AuthorId = 3, TopicId = 1 },
                new Book { Id = 5, Title = "Ar-Raheeq Al-Makhtum", BookCategoryId = 3, AuthorId = 4, TopicId = 4 },
                new Book { Id = 6, Title = "Bulugh al-Maram", BookCategoryId = 2, AuthorId = 5, TopicId = 3 },
                new Book { Id = 7, Title = "Don't Be Sad", BookCategoryId = 4, AuthorId = 6, TopicId = 6 }, // Mapped to 4
                new Book { Id = 8, Title = "Enjoy Your Life", BookCategoryId = 4, AuthorId = 7, TopicId = 6 }, // Mapped to 4
                new Book { Id = 9, Title = "Explanation of the Forty Nawawi Hadith - Part 1", BookCategoryId = 2, AuthorId = 8, TopicId = 3 },
        new Book { Id = 10, Title = "Explanation of the Forty Nawawi Hadith - Part 2", BookCategoryId = 3, AuthorId = 8, TopicId = 3 },
        new Book { Id = 11, Title = "Explanation of the Forty Nawawi Hadith - Part 3", BookCategoryId = 3, AuthorId = 8, TopicId = 3 },
        new Book { Id = 12, Title = "Fiqh Made Easy", BookCategoryId = 2, AuthorId = 9, TopicId = 2 },
        new Book { Id = 13, Title = "Fiqh Made Easy - Part 1", BookCategoryId = 2, AuthorId = 9, TopicId = 2 },
        new Book { Id = 14, Title = "Fiqh Made Easy - Part 2", BookCategoryId = 2, AuthorId = 9, TopicId = 2 },
        new Book { Id = 15, Title = "Fiqh Made Easy - Part 3", BookCategoryId = 2, AuthorId = 9, TopicId = 2 },
        new Book { Id = 16, Title = "Forty Hadith Nawawi", BookCategoryId = 3, AuthorId = 8, TopicId = 3 },
        // ... (continue for all books in the same pattern) ...
        new Book { Id = 66, Title = "When the Moon Split", BookCategoryId = 4, AuthorId = 4, TopicId = 4 }
    };

            await _context.Books.AddRangeAsync(books);
            await _context.SaveChangesAsync();
            return $"{books.Count} books have been successfully created/updated with author and topic.";
        }

        private async Task<string> SeedUsersAndProfilesAsync(string csvFilePath)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, csvFilePath);
            if (!System.IO.File.Exists(fullPath))
            {
                return $"Error: CSV file not found at {fullPath}.";
            }

            using var reader = new StreamReader(fullPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // We only need distinct users from the CSV file.
            var records = csv.GetRecords<UserDataRecord>()
                             .GroupBy(r => r.user_idx)
                             .Select(g => g.First())
                             .ToList();

            int createdCount = 0;
            int updatedCount = 0;

            foreach (var record in records)
            {
                var userEmail = $"user_{record.user_idx}@gmail.com";
                var user = await _userManager.FindByEmailAsync(userEmail);

                // If the user does not exist, create them.
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        FirstName = "User",
                        LastName = record.user_idx.ToString(),
                        EmailConfirmed = true // Confirm email automatically for seeded users.
                    };

                    // Create the user with a default, secure password.
                    var result = await _userManager.CreateAsync(user, "DefaultPassword123!");
                    if (!result.Succeeded)
                    {
                        // Handle user creation failure if necessary
                        continue; // Skip to the next record
                    }
                    createdCount++;
                }
                else
                {
                    updatedCount++;
                }

                // Update user profile with data from the CSV record.
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

            return $"{createdCount} new users created. {updatedCount} existing users updated.";
        }

        #region Private Helper Methods
        private string MapGender(int code) => code == 1 ? "Female" : "Male";
        private string MapCountry(int code) => $"Country_Code_{code}";
        private string MapEducation(int code) => $"Education_Level_{code}";
        private string MapReligiousLevel(int code) => $"Religious_Level_{code}";
        private string MapTopic(int code) => $"Topic_ID_{code}";

        // Private class to map the columns of the CSV file.
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
        #endregion
    }
}