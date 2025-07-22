using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.DataAccess.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; } // Can be used for "Country"

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        // --- NEW PROPERTIES FOR RECOMMENDATION MODEL ---

        [Display(Name = "Age")]
        public int? Age { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; } // e.g., "Male", "Female"

        [Display(Name = "Is New Muslim?")]
        public string? IsNewMuslim { get; set; } // e.g., "Yes", "No"

        [Display(Name = "Born Muslim?")]
        public string? BornMuslim { get; set; } // e.g., "Yes", "No"

        [Display(Name = "Education Level")]
        public string? EducationLevel { get; set; } // e.g., "Bachelor", "Master"

        [Display(Name = "Religious Level")]
        public string? ReligiousLevel { get; set; } // e.g., "Moderate", "Conservative"

        [Display(Name = "Preferred Topic")]
        public string? PreferredTopic { get; set; } // e.g., "Faith", "History"
    }
}