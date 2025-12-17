using System.ComponentModel.DataAnnotations;

namespace HotelmasCarga.Models
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; } = null!;

        [MaxLength(500)]
        public string? Direccion { get; set; }

        [MaxLength(50)]
        public string? Telefono { get; set; }
    }
}
