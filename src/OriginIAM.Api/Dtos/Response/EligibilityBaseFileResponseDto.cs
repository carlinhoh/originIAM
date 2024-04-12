namespace OriginIAM.Api.Dtos.Response
{
    /// <summary>
    /// Response DTO for eligibility file operations.
    /// </summary>
    public abstract class EligibilityBaseFileResponseDto
    {
        public string Message { get; set; }
        public string Errors { get; set; }
    }
}
