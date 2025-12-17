using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelmasCarga.Models
{
    public class Habitacion
    {
        [Key]
        public int IdHabitacion { get; set; }

        [Required]
        [MaxLength(10)]
        public string Numero { get; set; } = null!;

        public bool Activa { get; set; } = true;

        [Required]
        public int IdTipoHabitacion { get; set; }

        [ForeignKey(nameof(IdTipoHabitacion))]
        public TipoHabitacion? TipoHabitacion { get; set; }

        public ICollection<DisponibilidadHabitacion> Disponibilidades { get; set; } = new List<DisponibilidadHabitacion>();

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
