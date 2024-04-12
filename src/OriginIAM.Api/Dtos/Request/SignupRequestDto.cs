using System.ComponentModel.DataAnnotations;

namespace OriginIAM.Api.Dtos.Request
{
    public class SignupRequestDto
    {
        [Required(ErrorMessage = "Email adress is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, ErrorMessage = "The password must be between {2} and {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[\W_]).{8,}$", ErrorMessage = "Password must have at least 8 characters and include letters, numbers, and symbols.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Country code is required.")]
        [StringLength(2, ErrorMessage = "The country code must be {1} characters long.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "You must accept the terms and conditions.")]
        public bool AcceptTerms { get; set; }

        [StringLength(100, ErrorMessage = "The full name must not exceed {1} characters.")]
        public string FullName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
    }
}
