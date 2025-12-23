using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClinic.Models
{
    [Table("user_notifications")]
    public class UserNotification
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey("Notification")]
        [Column("notification_id")]
        public int NotificationId { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; }

        [Column("read_date")]
        public DateTime? ReadDate { get; set; }

        public virtual User User { get; set; }
        public virtual Notification Notification { get; set; }
    }
}