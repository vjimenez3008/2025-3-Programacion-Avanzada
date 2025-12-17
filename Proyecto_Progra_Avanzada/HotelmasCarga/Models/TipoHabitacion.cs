using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelmasCarga.Models
{
    public class TipoHabitacion
    {
        [Key]
        public int IdTipoHabitacion { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = null!;

        [MaxLength(200)]
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public ICollection<Habitacion> Habitaciones { get; set; } = new List<Habitacion>();

        public ICollection<ColaEspera> ColaEspera { get; set; } = new List<ColaEspera>();
    }
}
