using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Freelancer.Models
{
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }

        [Required]
        public int PayerId { get; set; }

        [ForeignKey("PayerId")]
        public User? Payer { get; set; }

        [Required]
        public int PayeeId { get; set; }

        [ForeignKey("PayeeId")]
        public User? Payee { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(255)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string TransactionId { get; set; } = string.Empty;

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        // NEW FIELD (for uploaded receipt image path)
        [StringLength(500)]
        public string? ReceiptImagePath { get; set; }
    }
}
