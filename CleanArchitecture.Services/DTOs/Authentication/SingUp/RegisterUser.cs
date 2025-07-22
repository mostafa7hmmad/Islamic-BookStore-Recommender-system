using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BrainHope.Services.DTO.Authentication.SingUp
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression(@"^[a-zA-Z]{2,20}$", ErrorMessage = "First name must be alphabetic and 2-20 characters long.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression(@"^[a-zA-Z]{2,20}$", ErrorMessage = "Last name must be alphabetic and 2-20 characters long.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression(@"^[a-zA-Z0-9_]{4,20}$", ErrorMessage = "Username must be 4-20 characters and can include letters, numbers, and underscores.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be at least 8 characters and include letters, numbers, and a special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? ProfilePicture { get; set; }

        [MaxLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
        public string? Bio { get; set; }


        [MaxLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
        public string? Location { get; set; }

    }

}
