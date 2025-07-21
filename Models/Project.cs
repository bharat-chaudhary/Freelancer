using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Freelancer.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Budget { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Start_Date { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime End_Date { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Bid_Date { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Open";

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int? TakenByFreelancerId { get; set; }

        [ForeignKey("TakenByFreelancerId")]
        public User? TakenByFreelancer { get; set; }

        [Required]
        [StringLength(50)]
        public string Language { get; set; }  // <-- Added this line

        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    }
}
