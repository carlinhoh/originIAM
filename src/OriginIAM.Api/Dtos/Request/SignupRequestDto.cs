using System.ComponentModel.DataAnnotations;

namespace OriginIAM.Api.Dtos.Request
{
    /// <summary>
    /// DTO for receiving user signup information.
    /// </summary>
    public class SignupRequestDto
    {
        /// <summary>
        /// The user's email address.
        /// </summary>
        [Required(ErrorMessage = "Email adress is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        /// <summary>
        /// The user's password.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, ErrorMessage = "The password must be between {2} and {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[\W_]).{8,}$", ErrorMessage = "Password must have at least 8 characters and include letters, numbers, and symbols.")]
        public string Password { get; set; }

        /// <summary>
        /// The user's country code.
        /// </summary>
        [Required(ErrorMessage = "Country code is required.")]
        [StringLength(2, ErrorMessage = "The country code must be {1} characters long.")]
        public string Country { get; set; }

        /// <summary>
        /// Indicates whether the user accepts the terms and conditions.
        /// </summary>
        [Required(ErrorMessage = "You must accept the terms and conditions.")]
        public bool AcceptTerms { get; set; }

        /// <summary>
        /// The user's full name.
        /// </summary>
        [StringLength(100, ErrorMessage = "The full name must not exceed {1} characters.")]
        public string FullName { get; set; }

        /// <summary>
        /// The user'd date of birth
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
    }
}
