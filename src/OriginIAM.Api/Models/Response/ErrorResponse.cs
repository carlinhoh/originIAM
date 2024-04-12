namespace OriginIAM.Api.Models.Response
{
    public class ErrorResponse
    {
        public string Message { get; set; } = "An unexpected error occurred during processing.";
        public List<string> Details { get; set; } = new List<string>();
    }
}
