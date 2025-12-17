using System;
using System.Collections.Generic;

namespace HotelmasCarga.Models
{
    public class RegistrarReservaViewModel
    {
        public string SearchName { get; set; }
        public int? TipoSeleccionado { get; set; }
        public List<Cliente> Clientes { get; set; } = new List<Cliente>();
        public List<TipoHabitacion> Tipos { get; set; } = new List<TipoHabitacion>();
        public List<DateTime> Dates { get; set; } = new List<DateTime>();
        public List<HabitacionAvailability> Rows { get; set; } = new List<HabitacionAvailability>();
    }

    public class HabitacionAvailability
    {
        public Habitacion Habitacion { get; set; }
        public Dictionary<DateTime, bool> AvailableByDate { get; set; } = new Dictionary<DateTime, bool>();
    }
}
