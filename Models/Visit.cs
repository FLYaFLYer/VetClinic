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
        [Range(20.0, 50.0, ErrorMessage = "Температура должна быть между 20 и 50 градусами")]
        public decimal? Temperature { get; set; }

        [Column("recommendations")]
        [StringLength(1000)]
        public string Recommendations { get; set; }

        [Column("next_visit_date")]
        public DateTime? NextVisitDate { get; set; }

        [Column("status")]
        [StringLength(50, ErrorMessage = "Статус не должен превышать 50 символов")]
        public string Status { get; set; } = "Запланирован";

        public virtual Patient Patient { get; set; }
        public virtual User User { get; set; }

        [NotMapped]
        public string TemperatureFormatted => Temperature.HasValue ? $"{Temperature.Value:N1}°C" : "Не измерялась";

        [NotMapped]
        public string WeightFormatted
        {
            get
            {
                if (Patient != null && Patient.Weight.HasValue)
                    return $"{Patient.Weight.Value:N2} кг";
                return "Не взвешивался";
            }
        }

        [NotMapped]
        public string NextVisitFormatted => NextVisitDate.HasValue ?
            NextVisitDate.Value.ToString("dd.MM.yyyy") : "Не назначен";

        [NotMapped]
        public bool IsCompleted => Status == "Завершён";

        [NotMapped]
        public bool CanBeCancelled => Status == "Запланирован" || Status == "В процессе";

        [NotMapped]
        public bool CanBeEdited => Status == "Запланирован" || Status == "В процессе";

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
                        else if (Diagnosis.Length > 500)
                            error = "Диагноз не должен превышать 500 символов";
                        break;

                    case nameof(Temperature):
                        if (Temperature.HasValue && (Temperature < 20 || Temperature > 50))
                            error = "Температура должна быть между 20 и 50 градусами";
                        break;

                    case nameof(Status):
                        if (string.IsNullOrWhiteSpace(Status))
                            error = "Выберите статус";
                        else if (Status.Length > 50)
                            error = "Статус не должен превышать 50 символов";
                        break;
                }

                return error;
            }
        }
    }
}