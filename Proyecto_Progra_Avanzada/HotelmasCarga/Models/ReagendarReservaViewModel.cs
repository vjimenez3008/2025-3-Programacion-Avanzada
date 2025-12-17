using System;
using System.Collections.Generic;

namespace HotelmasCarga.Models
{
    public class ReagendarReservaViewModel
    {
        public string? SearchName { get; set; }
        public List<Cliente> Clientes { get; set; } = new List<Cliente>();
        public int? SelectedClienteId { get; set; }

        public List<Reserva> Reservas { get; set; } = new List<Reserva>();
        public int? SelectedReservaId { get; set; }
        public Reserva? SelectedReserva { get; set; }

        public List<DateTime> Dates { get; set; } = new List<DateTime>();
        public Dictionary<DateTime, bool> AvailableByDate { get; set; } = new Dictionary<DateTime, bool>();
    }
}
