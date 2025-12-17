using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelmasCarga.Models
{
    public class HistorialReserva
    {
        [Key]
        public int IdHistorial { get; set; }

        [Required]
        public int IdReserva { get; set; }

        [ForeignKey(nameof(IdReserva))]
        public Reserva? Reserva { get; set; }

        [Required]
        public DateTime FechaAccion { get; set; }

        [Required]
        [MaxLength(20)]
        [RegularExpression("^(Creada|Reagendada|Cancelada)$")]
        public string Accion { get; set; } = null!;

        [MaxLength(200)]
        public string? Comentario { get; set; }
    }
}
