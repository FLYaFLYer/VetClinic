using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClinic.Models
{
    [Table("notifications")]
    public class Notification
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("Medicine")]
        [Column("medicine_id")]
        public int? MedicineId { get; set; }

        [Required]
        [StringLength(500)]
        [Column("message")]
        public string Message { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [NotMapped]
        public bool IsRead { get; set; }

        public virtual Medicine Medicine { get; set; }
    }
}