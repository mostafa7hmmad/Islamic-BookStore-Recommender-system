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

            string imagesFolder = Path.Combine(_env.WebRootPath, "images", "books");
            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            // Seeding books with new AuthorId and TopicId fields.
            // These are best-guess placeholders.
            var books = new List<Book>
            {
                // BookCategoryId: 1:Aqeedah, 2:Fiqh, 3:Hadith, 4:Seerah, 5:Tafsir, 6:General
                // TopicId is often related to the CategoryId in simple models.
                // AuthorId is a placeholder.
                new Book { Id = 0, Title = "Al-Wajiz in Fiqh", BookCategoryId = 1, AuthorId = 1, TopicId = 2, CoverImagePath = "/images/books/Al-Wajiz in Fiqh.jpg" },
                new Book { Id = 1, Title = "Aqeedah Wasitiyyah", BookCategoryId = 0, AuthorId = 2, TopicId = 1, CoverImagePath = "/images/books/Aqeedah Wasitiyyah.jpg" },
                new Book { Id = 2, Title = "Aqeedah at-Tahawiyyah - Part 1", BookCategoryId = 0, AuthorId = 3, TopicId = 1, CoverImagePath = "/images/books/Aqeedah at-Tahawiyyah - Part 1.jpg" },
                new Book { Id = 3, Title = "Aqeedah at-Tahawiyyah - Part 2", BookCategoryId = 0, AuthorId = 3, TopicId = 1, CoverImagePath = "/images/books/Aqeedah at-Tahawiyyah - Part 2.jpg" },
                new Book { Id = 4, Title = "Aqeedah at-Tahawiyyah - Part 3", BookCategoryId = 0, AuthorId = 3, TopicId = 1, CoverImagePath = "/images/books/Aqeedah at-Tahawiyyah - Part 3.jpg" },
                new Book { Id = 5, Title = "Ar-Raheeq Al-Makhtum", BookCategoryId = 3, AuthorId = 4, TopicId = 4, CoverImagePath = "/images/books/Ar-Raheeq Al-Makhtum.jpg" },
                new Book { Id = 6, Title = "Bulugh al-Maram", BookCategoryId = 2, AuthorId = 5, TopicId = 3, CoverImagePath = "/images/books/Bulugh al-Maram.jpg" },
                new Book { Id = 7, Title = "Don't Be Sad", BookCategoryId = 4, AuthorId = 6, TopicId = 6, CoverImagePath = "/images/books/Don't Be Sad.jpg" },
                new Book { Id = 8, Title = "Enjoy Your Life", BookCategoryId = 4, AuthorId = 7, TopicId = 6, CoverImagePath = "/images/books/Enjoy Your Life.jpg" },
                new Book { Id = 9, Title = "Explanation of the Forty Nawawi Hadith - Part 1", BookCategoryId = 2, AuthorId = 8, TopicId = 3, CoverImagePath = "/images/books/Explanation of the Forty Nawawi Hadith - Part 1.jpg" },
                new Book { Id = 10, Title = "Explanation of the Forty Nawawi Hadith - Part 2", BookCategoryId = 3, AuthorId = 8, TopicId = 3, CoverImagePath = "/images/books/Explanation of the Forty Nawawi Hadith - Part 2.jpg" },
                new Book { Id = 11, Title = "Explanation of the Forty Nawawi Hadith - Part 3", BookCategoryId = 3, AuthorId = 8, TopicId = 3, CoverImagePath = "/images/books/Explanation of the Forty Nawawi Hadith - Part 3.jpg" },
                new Book { Id = 12, Title = "Fiqh Made Easy", BookCategoryId = 2, AuthorId = 9, TopicId = 2, CoverImagePath = "/images/books/Fiqh Made Easy.jpg" },
                new Book { Id = 13, Title = "Fiqh Made Easy - Part 1", BookCategoryId = 2, AuthorId = 9, TopicId = 2, CoverImagePath = "/images/books/Fiqh Made Easy - Part 1.jpg" },
                new Book { Id = 14, Title = "Fiqh Made Easy - Part 2", BookCategoryId = 2, AuthorId = 9, TopicId = 2, CoverImagePath = "/images/books/Fiqh Made Easy - Part 2.jpg" },
                new Book { Id = 15, Title = "Fiqh Made Easy - Part 3", BookCategoryId = 2, AuthorId = 9, TopicId = 2, CoverImagePath = "/images/books/Fiqh Made Easy - Part 3.jpg" },
                new Book { Id = 16, Title = "Forty Hadith Nawawi", BookCategoryId = 3, AuthorId = 8, TopicId = 3, CoverImagePath = "/images/books/Forty Hadith Nawawi.jpg" },
                new Book { Id = 17, Title = "The Meaning of La ilaha illa Allah", BookCategoryId = 1, AuthorId = 2, TopicId = 1, CoverImagePath = "/images/books/Al-Wajiz in Fiqh.jpg" },
                new Book { Id = 18, Title = "Principles of Islamic Faith", BookCategoryId = 1, AuthorId = 2, TopicId = 1, CoverImagePath = "/images/books/Aqeedah Wasitiyyah.jpg" },
                new Book { Id = 19, Title = "The Names and Attributes of Allah", BookCategoryId = 1, AuthorId = 2, TopicId = 1, CoverImagePath = "/images/books/Aqeedah at-Tahawiyyah - Part 1.jpg" },
                new Book { Id = 20, Title = "Understanding Tawheed", BookCategoryId = 1, AuthorId = 2, TopicId = 1, CoverImagePath = "/images/books/Aqeedah at-Tahawiyyah - Part 2.jpg" },
                new Book { Id = 21, Title = "Major and Minor Shirk", BookCategoryId = 1, AuthorId = 2, TopicId = 1, CoverImagePath = "/images/books/Aqeedah at-Tahawiyyah - Part 3.jpg" },
                new Book { Id = 22, Title = "The Life of the Prophet", BookCategoryId = 3, AuthorId = 4, TopicId = 4, CoverImagePath = "/images/books/Ar-Raheeq Al-Makhtum.jpg" },
                new Book { Id = 23, Title = "Stories from Sahih al-Bukhari", BookCategoryId = 2, AuthorId = 5, TopicId = 3, CoverImagePath = "/images/books/Bulugh al-Maram.jpg" },
                new Book { Id = 24, Title = "Cure for the Anxious Heart", BookCategoryId = 4, AuthorId = 6, TopicId = 6, CoverImagePath = "/images/books/Don't Be Sad.jpg" },
                new Book { Id = 25, Title = "Etiquettes of Dealing with Others", BookCategoryId = 4, AuthorId = 7, TopicId = 6, CoverImagePath = "/images/books/Enjoy Your Life.jpg" },
                new Book { Id = 26, Title = "Commentary on Hadith 1–10", BookCategoryId = 2, AuthorId = 8, TopicId = 3, CoverImagePath = "/images/books/Explanation of the Forty Nawawi Hadith - Part 1.jpg" },
                new Book { Id = 27, Title = "Commentary on Hadith 11–20", BookCategoryId = 2, AuthorId = 8, TopicId = 3, CoverImagePath = "/images/books/Explanation of the Forty Nawawi Hadith - Part 2.jpg" },
                new Book { Id = 28, Title = "Commentary on Hadith 21–30", BookCategoryId = 2, AuthorId = 8, TopicId = 3, CoverImagePath = "/images/books/Explanation of the Forty Nawawi Hadith - Part 3.jpg" },
                new Book { Id = 29, Title = "Basics of Worship in Islam", BookCategoryId = 2, AuthorId = 9, TopicId = 2, CoverImagePath = "/images/books/Fiqh Made Easy.jpg" },
                new Book { Id = 30, Title = "Prayer According to Sunnah", BookCategoryId = 2, AuthorId = 9, TopicId = 2, CoverImagePath = "/images/books/Fiqh Made Easy - Part 1.jpg" },
                new Book { Id = 31, Title = "Zakah: Purity of Wealth", BookCategoryId = 2, AuthorId = 9, TopicId = 2, CoverImagePath = "/images/books/Fiqh Made Easy - Part 2.jpg" },
                new Book { Id = 32, Title = "Fasting in Ramadan", BookCategoryId = 2, AuthorId = 9, TopicId = 2, CoverImagePath = "/images/books/Fiqh Made Easy - Part 3.jpg" },
                new Book { Id = 33, Title = "The Final Day: Belief in the Hereafter", BookCategoryId = 1, AuthorId = 10, TopicId = 1, CoverImagePath = "/images/books/Al-Wajiz in Fiqh.jpg" },
                new Book { Id = 34, Title = "Angels in Islam", BookCategoryId = 1, AuthorId = 10, TopicId = 1, CoverImagePath = "/images/books/Aqeedah Wasitiyyah.jpg" },
                new Book { Id = 35, Title = "Jinn and Human Interaction", BookCategoryId = 1, AuthorId = 10, TopicId = 1, CoverImagePath = "/images/books/Aqeedah at-Tahawiyyah - Part 1.jpg" },
                new Book { Id = 36, Title = "The Biography of Abu Bakr", BookCategoryId = 3, AuthorId = 4, TopicId = 4, CoverImagePath = "/images/books/Ar-Raheeq Al-Makhtum.jpg" },
                new Book { Id = 37, Title = "Women in the Qur'an", BookCategoryId = 4, AuthorId = 6, TopicId = 6, CoverImagePath = "/images/books/Don't Be Sad.jpg" },
                new Book { Id = 38, Title = "Youth and Islam", BookCategoryId = 4, AuthorId = 7, TopicId = 6, CoverImagePath = "/images/books/Enjoy Your Life.jpg" },
                new Book { Id = 39, Title = "Understanding Hadith Terminology", BookCategoryId = 2, AuthorId = 8, TopicId = 3, CoverImagePath = "/images/books/Explanation of the Forty Nawawi Hadith - Part 1.jpg" },
                new Book { Id = 40, Title = "Manners of the Prophet", BookCategoryId = 3, AuthorId = 4, TopicId = 4, CoverImagePath = "/images/books/Explanation of the Forty Nawawi Hadith - Part 2.jpg" },
                new Book { Id = 41, Title = "Salah: The Believer's Ascension", BookCategoryId = 2, AuthorId = 9, TopicId = 2, CoverImagePath = "/images/books/Fiqh Made Easy.jpg" },
                new Book { Id = 42, Title = "Unity and Brotherhood in Islam", BookCategoryId = 4, AuthorId = 6, TopicId = 6, CoverImagePath = "/images/books/Don't Be Sad.jpg" },
                new Book { Id = 43, Title = "Islamic Morals & Manners", BookCategoryId = 4, AuthorId = 7, TopicId = 6, CoverImagePath = "/images/books/Enjoy Your Life.jpg" },
                new Book { Id = 44, Title = "Inspiring Quranic Stories", BookCategoryId = 4, AuthorId = 12, TopicId = 5, CoverImagePath = "/images/books/Aqeedah at-Tahawiyyah - Part 3.jpg" },
                new Book { Id = 45, Title = "Lives of the Prophets", BookCategoryId = 3, AuthorId = 4, TopicId = 4, CoverImagePath = "/images/books/Ar-Raheeq Al-Makhtum.jpg" },
                new Book { Id = 46, Title = "Lessons from Hijrah", BookCategoryId = 3, AuthorId = 4, TopicId = 4, CoverImagePath = "/images/books/Explanation of the Forty Nawawi Hadith - Part 3.jpg" },
                new Book { Id = 47, Title = "Faith in Times of Fitnah", BookCategoryId = 1, AuthorId = 2, TopicId = 1, CoverImagePath = "/images/books/Aqeedah Wasitiyyah.jpg" },
                new Book { Id = 48, Title = "Spiritual Cleanliness", BookCategoryId = 4, AuthorId = 6, TopicId = 6, CoverImagePath = "/images/books/Fiqh Made Easy - Part 1.jpg" },
                new Book { Id = 49, Title = "Friday Sermons by the Prophet", BookCategoryId = 2, AuthorId = 8, TopicId = 3, CoverImagePath = "/images/books/Bulugh al-Maram.jpg" },
                new Book { Id = 50, Title = "Duties of the Muslim Woman", BookCategoryId = 4, AuthorId = 7, TopicId = 6, CoverImagePath = "/images/books/Fiqh Made Easy - Part 2.jpg" },
                new Book { Id = 51, Title = "Signs of the Last Day", BookCategoryId = 1, AuthorId = 10, TopicId = 1, CoverImagePath = "/images/books/Fiqh Made Easy - Part 3.jpg" },
                new Book { Id = 52, Title = "Calling to Allah: Methods and Etiquette", BookCategoryId = 4, AuthorId = 11, TopicId = 6, CoverImagePath = "/images/books/Enjoy Your Life.jpg" },
                new Book { Id = 53, Title = "Jurisprudence of Worship", BookCategoryId = 2, AuthorId = 9, TopicId = 2, CoverImagePath = "/images/books/Al-Wajiz in Fiqh.jpg" },
                new Book { Id = 54, Title = "Biography of Umar ibn al-Khattab", BookCategoryId = 3, AuthorId = 4, TopicId = 4, CoverImagePath = "/images/books/Ar-Raheeq Al-Makhtum.jpg" },
                new Book { Id = 55, Title = "Biography of Uthman ibn Affan", BookCategoryId = 3, AuthorId = 4, TopicId = 4, CoverImagePath = "/images/books/Aqeedah at-Tahawiyyah - Part 2.jpg" },
                new Book { Id = 56, Title = "Biography of Ali ibn Abi Talib", BookCategoryId = 3, AuthorId = 4, TopicId = 4, CoverImagePath = "/images/books/Aqeedah at-Tahawiyyah - Part 1.jpg" },
                new Book { Id = 57, Title = "Du'a from the Quran and Sunnah", BookCategoryId = 4, AuthorId = 7, TopicId = 6, CoverImagePath = "/images/books/Explanation of the Forty Nawawi Hadith - Part 3.jpg" },
                new Book { Id = 58, Title = "How to Protect Your Iman", BookCategoryId = 1, AuthorId = 2, TopicId = 1, CoverImagePath = "/images/books/Aqeedah Wasitiyyah.jpg" },
                new Book { Id = 59, Title = "The Path to Paradise", BookCategoryId = 4, AuthorId = 6, TopicId = 6, CoverImagePath = "/images/books/Don't Be Sad.jpg" },
                new Book { Id = 60, Title = "Building a Muslim Home", BookCategoryId = 4, AuthorId = 6, TopicId = 6, CoverImagePath = "/images/books/Fiqh Made Easy.jpg" },
                new Book { Id = 61, Title = "Wisdom of the Prophet", BookCategoryId = 3, AuthorId = 4, TopicId = 4, CoverImagePath = "/images/books/Explanation of the Forty Nawawi Hadith - Part 1.jpg" },
                new Book { Id = 62, Title = "Life After Death in Islam", BookCategoryId = 1, AuthorId = 10, TopicId = 1, CoverImagePath = "/images/books/Aqeedah at-Tahawiyyah - Part 3.jpg" },
                new Book { Id = 63, Title = "The Quran and Modern Science", BookCategoryId = 4, AuthorId = 12, TopicId = 5, CoverImagePath = "/images/books/Fiqh Made Easy - Part 1.jpg" },
                new Book { Id = 64, Title = "Understanding Islamic Law", BookCategoryId = 2, AuthorId = 9, TopicId = 2, CoverImagePath = "/images/books/Fiqh Made Easy - Part 2.jpg" },
                new Book { Id = 65, Title = "The Manners of the Salaf", BookCategoryId = 4, AuthorId = 7, TopicId = 6, CoverImagePath = "/images/books/Fiqh Made Easy - Part 3.jpg" },
                new Book { Id = 66, Title = "When the Moon Split", BookCategoryId = 4, AuthorId = 4, TopicId = 4, CoverImagePath = "/images/books/When the Moon Split.jpg" }
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

        #region helper methods
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
            #endregion
        }
    }
}
