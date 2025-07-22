using CleanArchitecture.DataAccess.Contexts;
using CleanArchitecture.DataAccess.Models;
using CleanArchitecture.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace CleanArchitecture.Services.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly InferenceSession _session;
        private readonly ApplicationDbContext _context;

        public RecommendationService(InferenceSession session, ApplicationDbContext context)
        {
            _session = session;
            _context = context;
        }

        public List<long> GetRecommendations(ApplicationUser user)
        {
            // 1. Get all books from the database.
            var allBooks = _context.Books.AsNoTracking().ToList();
            var predictedRatings = new List<(long BookId, float PredictedRating)>();

            // 2. Loop through each book to predict a rating for the current user.
            foreach (var book in allBooks)
            {
                // 3. Construct the list of named inputs required by the model.
                // The names MUST EXACTLY match the names from the Netron screenshot.
                var inputs = new List<NamedOnnxValue>
                {
                    // --- Book-specific inputs ---
                    NamedOnnxValue.CreateFromTensor("book_idx_in", new DenseTensor<int>(new int[] { book.Id }, new[] { 1, 1 })),
                    NamedOnnxValue.CreateFromTensor("topic_idx_in", new DenseTensor<int>(new int[] { book.TopicId }, new[] { 1, 1 })),
                    NamedOnnxValue.CreateFromTensor("author_idx_in", new DenseTensor<int>(new int[] { book.AuthorId }, new[] { 1, 1 })),
                    NamedOnnxValue.CreateFromTensor("category_in", new DenseTensor<int>(new int[] { book.BookCategoryId }, new[] { 1, 1 })),

                    // --- User-specific inputs ---
                    NamedOnnxValue.CreateFromTensor("country_in", new DenseTensor<int>(new int[] { MapLocationToInt(user.Location) }, new[] { 1, 1 })),
                    NamedOnnxValue.CreateFromTensor("gender_in", new DenseTensor<int>(new int[] { MapGenderToInt(user.Gender) }, new[] { 1, 1 })),
                    NamedOnnxValue.CreateFromTensor("is_new_muslim_in", new DenseTensor<int>(new int[] { MapYesNoToInt(user.IsNewMuslim) }, new[] { 1, 1 })),
                    NamedOnnxValue.CreateFromTensor("born_muslim_in", new DenseTensor<int>(new int[] { MapYesNoToInt(user.BornMuslim) }, new[] { 1, 1 })),
                    NamedOnnxValue.CreateFromTensor("education_level_in", new DenseTensor<int>(new int[] { MapEducationToInt(user.EducationLevel) }, new[] { 1, 1 })),
                    NamedOnnxValue.CreateFromTensor("religious_level_in", new DenseTensor<int>(new int[] { MapReligiousLevelToInt(user.ReligiousLevel) }, new[] { 1, 1 })),
                    
                    // --- Placeholder for 'num_in' ---
                    // The model expects a float32 tensor of shape [1, 3] for this input.
                    // Its exact contents are unknown, so we use placeholders. This is a critical
                    // area to verify with the person who trained the model.
                    NamedOnnxValue.CreateFromTensor("num_in", new DenseTensor<float>(new float[] { 0.5f, 10.0f, 0.0f }, new[] { 1, 3 }))
                };

                // 4. Run inference to get the predicted rating for this single book.
                using (var results = _session.Run(inputs))
                {
                    var ratingTensor = results.FirstOrDefault(r => r.Name == "rating")?.AsTensor<float>();
                    if (ratingTensor != null && ratingTensor.Length > 0)
                    {
                        predictedRatings.Add((book.Id, ratingTensor.First()));
                    }
                }
            }

            // 5. Sort all the predicted ratings and return the IDs of the top 5 books.
            var recommendedBookIds = predictedRatings
                .OrderByDescending(r => r.PredictedRating)
                .Take(5)
                .Select(r => (long)r.BookId)
                .ToList();

            return recommendedBookIds;
        }

        #region Private Mapping Helpers
        private int MapGenderToInt(string gender) => gender?.ToLower() == "female" ? 1 : 0;
        private int MapYesNoToInt(string value) => value?.ToLower() == "yes" ? 1 : 0;
        private int MapLocationToInt(string location) => location switch { "Country_Code_7" => 7, "Country_Code_6" => 6, _ => 0 };
        private int MapEducationToInt(string level) => level switch { "Education_Level_2" => 2, _ => 0 };
        private int MapReligiousLevelToInt(string level) => level switch { "Religious_Level_2" => 2, "Religious_Level_0" => 0, _ => 0 };
        private int MapTopicToInt(string topic) => topic switch { "Topic_ID_16" => 16, "Topic_ID_6" => 6, _ => 0 };
        #endregion
    }
}