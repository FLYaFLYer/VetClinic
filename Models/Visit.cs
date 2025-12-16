using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClinic.Models
{
    [Table("visits")]
    public class Visit : IDataErrorInfo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("Patient")]
        [Column("patient_id")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите пациента")]
        public int PatientId { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("visit_date")]
        public DateTime VisitDate { get; set; }

        [Required(ErrorMessage = "Диагноз обязателен")]
        [StringLength(500, ErrorMessage = "Диагноз не должен превышать 500 символов")]
        [Column("diagnosis")]
        public string Diagnosis { get; set; }

        [Column("symptoms")]
        [StringLength(1000)]
        public string Symptoms { get; set; }

        [Column("temperature")]
        [Range(30.0, 45.0, ErrorMessage = "Температура должна быть между 30 и 45 градусами")]
        public decimal? Temperature { get; set; }

        [Column("weight")]
        [Range(0.01, 1000.0, ErrorMessage = "Вес должен быть положительным числом (0.01 - 1000)")]
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

        // Реализация IDataErrorInfo
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(PatientId):
                        if (PatientId <= 0)
                            error = "Выберите пациента";
                        break;

                    case nameof(Diagnosis):
                        if (string.IsNullOrWhiteSpace(Diagnosis))
                            error = "Введите диагноз";
                        break;

                    case nameof(Temperature):
                        if (Temperature.HasValue && (Temperature < 30 || Temperature > 45))
                            error = "Температура должна быть между 30 и 45 градусами";
                        break;

                    case nameof(Weight):
                        if (Weight.HasValue && Weight <= 0)
                            error = "Вес должен быть положительным числом";
                        break;
                }

                return error;
            }
        }
    }
}