using System;

namespace OriginIAM.Domain.Entities
{
    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Country { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public decimal? Salary { get; set; }
        public string EmployerId { get; set; }
        public bool IsActive { get; set; }
        public bool VerifiedEmployee { get; set; }
        public DateTime CreatedDate { get; set; }


        public User()
        {
        }

        public User(string email, string passwordHash, string country, string employerId)
        {
            Id = Guid.NewGuid().ToString();
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            Country = country ?? throw new ArgumentNullException(nameof(country));
            IsActive = true;
            EmployerId = employerId;
        }

        public User(string email, string passwordHash, string country, string employerId, string fullName)
        {
            Id = Guid.NewGuid().ToString();
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            Country = country ?? throw new ArgumentNullException(nameof(country));
            IsActive = true;
            EmployerId = employerId;
        }

        public User(string email, string fullName, string country, DateTime birthDate, decimal salary, string employerId)
        {
            Id = Guid.NewGuid().ToString();
            Email = email ?? throw new ArgumentNullException(nameof(email));
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            Country = country ?? throw new ArgumentNullException(nameof(country));
            BirthDate = birthDate;
            IsActive = true;
            EmployerId = employerId;
        }
    }
}
