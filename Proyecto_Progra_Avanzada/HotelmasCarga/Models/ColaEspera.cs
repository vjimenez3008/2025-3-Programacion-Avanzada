using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelmasCarga.Models
{
    public class ColaEspera
    {
        [Key]
        public int IdCola { get; set; }

        [Required]
        public int IdCliente { get; set; }

        [ForeignKey(nameof(IdCliente))]
        public Cliente? Cliente { get; set; }

        [Required]
        public int IdTipoHabitacion { get; set; }

        [ForeignKey(nameof(IdTipoHabitacion))]
        public TipoHabitacion? TipoHabitacion { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime FechaDeseada { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        [MaxLength(20)]
        [RegularExpression("^(Activo|Atendido|Descartado)$")]
        public string Estado { get; set; } = null!;
    }
}
