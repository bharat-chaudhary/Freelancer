using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Freelancer.Models
{
    public class Freelancers
    {
      
            [Key]
            public int FreelancerID { get; set; }

            // Foreign Key to User table
            [Required]
            public int UserId { get; set; }

            [ForeignKey("UserId")]
            public User User { get; set; }

            [Required]
            public string FullName { get; set; }

            public string Skills { get; set; }

            public string Bio { get; set; }

            public DateTime JoinedDate { get; set; } = DateTime.Now;

            // Navigation property for Bids
           
        }
    }



