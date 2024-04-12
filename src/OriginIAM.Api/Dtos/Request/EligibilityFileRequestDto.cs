using System.ComponentModel.DataAnnotations;

namespace OriginIAM.Api.Dtos.Request
{
    /// <summary>
    /// DTO for eligibility file request.
    /// </summary>
    public class EligibilityFileRequestDto
    {
        /// <summary>
        /// URL of the file to be processed.
        /// </summary>
        [Required(ErrorMessage = "The file URL is required.")]
        [Url(ErrorMessage = "The file field must be a valid URL.")]
        public string FileAddress { get; set; }
        
        /// <summary>
        /// Name of the employer related to the file.
        /// </summary>
        [Required(ErrorMessage = "The employer name is required.")]
        public string EmployerName { get; set; }
    }
}
