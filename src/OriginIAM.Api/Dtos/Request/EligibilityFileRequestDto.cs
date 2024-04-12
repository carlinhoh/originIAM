using System.ComponentModel.DataAnnotations;

namespace OriginIAM.Api.Dtos.Request
{
    public class EligibilityFileRequestDto
    {
        [Required(ErrorMessage = "The file URL is required.")]
        [Url(ErrorMessage = "The file field must be a valid URL.")]
        public string FileAddress { get; set; }

        [Required(ErrorMessage = "The employer name is required.")]
        public string EmployerName { get; set; }
    }
}
