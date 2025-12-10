using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClinic.Models
{
    [Table("medicine_stocks")]
    public class MedicineStock
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("Medicine")]
        [Column("medicine_id")]
        public int MedicineId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("location")]
        [StringLength(50)]
        public string Location { get; set; }

        [Column("expiry_date")]
        public DateTime? ExpiryDate { get; set; }

        public virtual Medicine Medicine { get; set; }
    }
}