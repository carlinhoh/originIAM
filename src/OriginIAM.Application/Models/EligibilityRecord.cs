namespace OriginIAM.Application.Models
{
    public class EligibilityRecord
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Country { get; set; }
        public DateTime? BirthDate { get; set; }
        public decimal? Salary { get; set; }

        public bool ProcessSuccess { get; set; }
        public List<string> Errors { get; set; }
    }

}