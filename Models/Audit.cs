using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Freelancer.Models
{
    public class Audit
    {
        [Key]
        public int Audit_id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int User_id { get; set; }

        public User User { get; set; }

        [Required]
        public DateTime Login_time { get; set; } = DateTime.Now;

        public DateTime? Logout_time { get; set; }
    }
}
