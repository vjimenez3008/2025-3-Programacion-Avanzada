using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelmasCarga.Models
{
    public class Reserva
    {
        [Key]
        public int IdReserva { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime FechaReserva { get; set; }

        [Required]
        [MaxLength(20)]
        [RegularExpression("^(Activa|Cancelada|Reagendada)$")]
        public string Estado { get; set; } = null!;

        [Required]
        public int IdCliente { get; set; }

        [ForeignKey(nameof(IdCliente))]
        public Cliente? Cliente { get; set; }

        [Required]
        public int IdHabitacion { get; set; }

        [ForeignKey(nameof(IdHabitacion))]
        public Habitacion? Habitacion { get; set; }

        public ICollection<HistorialReserva> Historial { get; set; } = new List<HistorialReserva>();
    }
}
