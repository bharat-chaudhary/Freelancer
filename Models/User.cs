using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Freelancer.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }  // Admin, Freelancer, Employer

        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? ContactNumber { get; set; }
        public string? ProfilePicture { get; set; }

        public string? Skills { get; set; } // New Skill field (comma-separated string)
        public decimal? HourlyRate { get; set; } // New Hourly Rate field

        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
        public ICollection<Payment> PaymentsMade { get; set; } = new List<Payment>();
        public ICollection<Payment> PaymentsReceived { get; set; } = new List<Payment>();
    }
}
