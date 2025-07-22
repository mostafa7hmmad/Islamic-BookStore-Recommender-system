using CleanArchitecture.DataAccess.Models;
using CleanArchitecture.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanArchitecture.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IBookService _bookService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RecommendationController(
            IRecommendationService recommendationService,
            IBookService bookService,
            UserManager<ApplicationUser> userManager)
        {
            _recommendationService = recommendationService;
            _bookService = bookService;
            _userManager = userManager;
        }

        [HttpGet("GetMyRecommendedBooks")]
        public async Task<IActionResult> GetMyRecommendedBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            // Check for incomplete profile
            if (user.Age == null || string.IsNullOrEmpty(user.Gender) || string.IsNullOrEmpty(user.Location))
            {
                return BadRequest(new { Message = "Your recommendation profile is incomplete. Please update your profile." });
            }

            // The controller's job is now simple: just pass the user object to the service.
            // All the complex mapping and tensor creation is handled in the RecommendationService.
            var recommendedBookIds = _recommendationService.GetRecommendations(user);

            if (recommendedBookIds == null || recommendedBookIds.Count == 0)
            {
                return Ok(new { Message = "No specific recommendations found. Here are some popular books.", Data = await _bookService.GetAllBooksAsync() });
            }

            var recommendedBooks = await _bookService.GetBooksByIdsAsync(recommendedBookIds);
            return Ok(recommendedBooks);
        }
    }
}