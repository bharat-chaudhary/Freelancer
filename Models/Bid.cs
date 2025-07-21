using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Freelancer.Models
{
    public class Bid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Bid_ID { get; set; }

        [Required]
        public int Project_ID { get; set; }

        [ForeignKey("Project_ID")]
        public Project? Project { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal BidAmount { get; set; }

        [Required]
        public DateTime Bid_Date { get; set; }

        [Required]
        [StringLength(10)]
        public string Assign { get; set; } = "No";

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
