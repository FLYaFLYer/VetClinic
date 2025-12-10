using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClinic.Models
{
    [Table("medicines")]
    public class Medicine
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }

        [Column("category")]
        [StringLength(50)]
        public string Category { get; set; }

        [Required]
        [StringLength(20)]
        [Column("unit")]
        public string Unit { get; set; } = "шт";

        [Column("price")]
        public decimal Price { get; set; }

        [Column("min_stock")]
        public int MinStock { get; set; } = 10;

        [NotMapped]
        public int TotalQuantity { get; set; }

        [NotMapped]
        public bool IsLowStock => TotalQuantity < MinStock;

        [NotMapped]
        public string StockStatus => IsLowStock ? "Мало!" : "В норме";

        [NotMapped]
        public string PriceFormatted => $"{Price:N2} ₽";
    }
}