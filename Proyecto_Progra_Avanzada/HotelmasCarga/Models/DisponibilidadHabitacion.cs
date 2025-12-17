using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelmasCarga.Models
{
    public class DisponibilidadHabitacion
    {
        [Key]
        public int IdDisponibilidad { get; set; }
        [Required]
        [Column(TypeName = "date")]
        public DateTime Fecha { get; set; }

        public bool EstaDisponible { get; set; }

        [Required]
        public int IdHabitacion { get; set; }

        [ForeignKey(nameof(IdHabitacion))]
        public Habitacion? Habitacion { get; set; }
    }
}
