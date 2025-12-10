using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClinic.Models
{
    [Table("prescriptions")]
    public class Prescription
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("Visit")]
        [Column("visit_id")]
        public int VisitId { get; set; }

        [ForeignKey("Medicine")]
        [Column("medicine_id")]
        public int MedicineId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("dosage")]
        public string Dosage { get; set; }

        [Required]
        [StringLength(50)]
        [Column("frequency")]
        public string Frequency { get; set; }

        [Column("duration_days")]
        public int DurationDays { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        public virtual Visit Visit { get; set; }
        public virtual Medicine Medicine { get; set; }
    }
}