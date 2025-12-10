using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClinic.Models
{
    [Table("owners")]
    public class Owner
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("first_name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Column("last_name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(20)]
        [Column("phone")]
        public string Phone { get; set; }

        [StringLength(100)]
        [Column("email")]
        public string Email { get; set; }

        [StringLength(200)]
        [Column("address")]
        public string Address { get; set; }

        [NotMapped]
        public string FullName => $"{LastName} {FirstName}";

        [NotMapped]
        public string ContactInfo => $"{Phone}{(string.IsNullOrEmpty(Email) ? "" : $", {Email}")}";
    }
}