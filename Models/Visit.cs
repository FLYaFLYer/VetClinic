using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClinic.Models
{
    [Table("visits")]
    public class Visit
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("Patient")]
        [Column("patient_id")]
        public int PatientId { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("visit_date")]
        public DateTime VisitDate { get; set; }

        [Required]
        [StringLength(500)]
        [Column("diagnosis")]
        public string Diagnosis { get; set; }

        [Column("symptoms")]
        [StringLength(1000)]
        public string Symptoms { get; set; }

        [Column("temperature")]
        public decimal? Temperature { get; set; }

        [Column("weight")]
        public decimal? Weight { get; set; }

        [Column("recommendations")]
        [StringLength(1000)]
        public string Recommendations { get; set; }

        [Column("next_visit_date")]
        public DateTime? NextVisitDate { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Завершён";

        public virtual Patient Patient { get; set; }
        public virtual User User { get; set; }

        [NotMapped]
        public string TemperatureFormatted => Temperature.HasValue ? $"{Temperature.Value:N1}°C" : "Не измерялась";

        [NotMapped]
        public string WeightFormatted => Weight.HasValue ? $"{Weight.Value:N2} кг" : "Не взвешивался";

        [NotMapped]
        public string NextVisitFormatted => NextVisitDate.HasValue ?
            NextVisitDate.Value.ToString("dd.MM.yyyy") : "Не назначен";
    }
}