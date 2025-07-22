using CleanArchitecture.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataSeedingController : ControllerBase
    {
        private readonly IDataSeedingService _dataSeeder;

        public DataSeedingController(IDataSeedingService dataSeeder)
        {
            _dataSeeder = dataSeeder;
        }

        [HttpPost("SeedInitialData")]
        public async Task<IActionResult> SeedInitialData()
        {
            try
            {
                var (categories, books, users) = await _dataSeeder.SeedAllAsync("DataFiles/data.csv");
                return Ok(new
                {
                    Message = "Seeding completed.",
                    Categories = categories,
                    Books = books,
                    Users = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error during seeding: {ex.Message}");
            }
        }
    }
}