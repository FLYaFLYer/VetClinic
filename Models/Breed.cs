using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClinic.Models
{
    [Table("breeds")]
    public class Breed
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("AnimalType")]
        [Column("animal_type_id")]
        public int AnimalTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("name")]
        public string Name { get; set; }

        public virtual AnimalType AnimalType { get; set; }
    }
}