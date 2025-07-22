using CleanArchitecture.DataAccess.Models; // Add this using

namespace CleanArchitecture.Services.Interfaces
{
    public interface IRecommendationService
    {
        // Change the input from a Tensor to the ApplicationUser object itself
        List<long> GetRecommendations(ApplicationUser user);
    }
}