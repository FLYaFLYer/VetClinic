using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VetClinic.Models;

namespace VetClinic.Models
{
    [Table("patients")]
    public class Patient
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; }

        [ForeignKey("Owner")]
        [Column("owner_id")]
        public int OwnerId { get; set; }

        [ForeignKey("AnimalType")]
        [Column("animal_type_id")]
        public int AnimalTypeId { get; set; }

        [ForeignKey("Breed")]
        [Column("breed_id")]
        public int? BreedId { get; set; }

        [Column("birth_date")]
        public DateTime? BirthDate { get; set; }

        [Column("weight")]
        public decimal? Weight { get; set; }

        [Column("color")]
        [StringLength(50)]
        public string Color { get; set; }

        [Column("distinctive_features")]
        [StringLength(500)]
        public string DistinctiveFeatures { get; set; }

        [Column("avatar_path")]
        [StringLength(500)]
        public string AvatarPath { get; set; }

        [Column("chip_number")]
        [StringLength(50)]
        public string ChipNumber { get; set; }

        public virtual Owner Owner { get; set; }
        public virtual AnimalType AnimalType { get; set; }
        public virtual Breed Breed { get; set; }

        [NotMapped]
        public int Age
        {
            get
            {
                if (!BirthDate.HasValue) return 0;
                var today = DateTime.Today;
                var age = today.Year - BirthDate.Value.Year;
                if (BirthDate.Value.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        [NotMapped]
        public string AgeFormatted => BirthDate.HasValue ? $"{Age} лет/год(а)" : "Не указан";
    }
}