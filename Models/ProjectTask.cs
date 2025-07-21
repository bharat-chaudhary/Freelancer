using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Freelancer.Models
{
    [Table("Tasks")]
    public class ProjectTask
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [EnumDataType(typeof(TaskStatus))]
        public TaskStatus Status { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CompletionDate { get; set; }

        [StringLength(1000)]
        public string TaskDescription { get; set; }

        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }

    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Completed,
        OnHold,
        Cancelled
    }
}
