using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelmasCarga.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

        public ICollection<ColaEspera> ColaEspera { get; set; } = new List<ColaEspera>();
    }
}
