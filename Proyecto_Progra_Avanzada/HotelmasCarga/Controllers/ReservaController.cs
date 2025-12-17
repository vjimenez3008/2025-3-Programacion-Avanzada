using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelmasCarga.Data;
using HotelmasCarga.Models;

namespace HotelmasCarga.Controllers
{
    public class ReservaController : Controller
    {
        private readonly HotelmasCargaContext _context;
        public ReservaController(HotelmasCargaContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var reservas = _context.Reservas.Include(r => r.Cliente).Include(r => r.Habitacion);
            return View(await reservas.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var reserva = await _context.Reservas.Include(r => r.Cliente).Include(r => r.Habitacion).FirstOrDefaultAsync(r => r.IdReserva == id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        public IActionResult Create()
        {
            ViewData["Clientes"] = new SelectList(_context.Clientes, "IdCliente", "Nombre");
            ViewData["Habitaciones"] = new SelectList(_context.Habitaciones.Where(h => h.Activa), "IdHabitacion", "Numero");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FechaReserva,Estado,IdCliente,IdHabitacion")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Clientes"] = new SelectList(_context.Clientes, "IdCliente", "Nombre", reserva.IdCliente);
            ViewData["Habitaciones"] = new SelectList(_context.Habitaciones.Where(h => h.Activa), "IdHabitacion", "Numero", reserva.IdHabitacion);
            return View(reserva);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound();
            ViewData["Clientes"] = new SelectList(_context.Clientes, "IdCliente", "Nombre", reserva.IdCliente);
            ViewData["Habitaciones"] = new SelectList(_context.Habitaciones.Where(h => h.Activa), "IdHabitacion", "Numero", reserva.IdHabitacion);
            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdReserva,FechaReserva,Estado,IdCliente,IdHabitacion")] Reserva reserva)
        {
            if (id != reserva.IdReserva) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Clientes"] = new SelectList(_context.Clientes, "IdCliente", "Nombre", reserva.IdCliente);
            ViewData["Habitaciones"] = new SelectList(_context.Habitaciones.Where(h => h.Activa), "IdHabitacion", "Numero", reserva.IdHabitacion);
            return View(reserva);
        }

        // Registrar: display search, calendar of availability and allow selection
        [HttpGet]
        [HotelmasCarga.Filters.RequireRole(HotelmasCarga.Models.Roles.AgendarReservas)]
        public IActionResult Registrar(string searchName, int? tipoId)
        {
            var vm = new Models.RegistrarReservaViewModel();
            vm.SearchName = searchName;
            vm.TipoSeleccionado = tipoId;

            // next 14 days
            var dates = new List<DateTime>();
            for (int i = 0; i < 14; i++) dates.Add(DateTime.Today.AddDays(i));
            vm.Dates = dates;

            vm.Tipos = _context.TiposHabitacion.ToList();

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                vm.Clientes = _context.Clientes.Where(c => c.Nombre.Contains(searchName)).Take(20).ToList();
            }

            var habitacionesQuery = _context.Habitaciones.Include(h => h.TipoHabitacion).AsQueryable();
            if (tipoId.HasValue) habitacionesQuery = habitacionesQuery.Where(h => h.IdTipoHabitacion == tipoId.Value);
            var habitaciones = habitacionesQuery.ToList();

            foreach (var h in habitaciones)
            {
                var row = new Models.HabitacionAvailability { Habitacion = h };
                // fetch disponibilidades for these dates
                var fechas = vm.Dates;
                var disps = _context.Disponibilidades.Where(d => d.IdHabitacion == h.IdHabitacion && fechas.Contains(d.Fecha)).ToList();
                foreach (var d in fechas)
                {
                    var disp = disps.FirstOrDefault(x => x.Fecha.Date == d.Date);
                    // available if no record or record indicates available
                    var available = disp == null || disp.EstaDisponible;
                    row.AvailableByDate[d] = available;
                }
                vm.Rows.Add(row);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HotelmasCarga.Filters.RequireRole(HotelmasCarga.Models.Roles.AgendarReservas)]
        public async Task<IActionResult> RegistrarConfirm(int clienteId, string selectedCell, int? tipoId, string searchName)
        {
            // selectedCell format: "{habitacionId}|yyyy-MM-dd"
            if (clienteId <= 0 || string.IsNullOrWhiteSpace(selectedCell))
            {
                ModelState.AddModelError(string.Empty, "Seleccione un cliente y una habitación/fecha disponible.");
                return RedirectToAction(nameof(Registrar), new { searchName, tipoId });
            }

            var parts = selectedCell.Split('|');
            if (parts.Length != 2 || !int.TryParse(parts[0], out var habitacionId) || !DateTime.TryParse(parts[1], out var fecha))
            {
                ModelState.AddModelError(string.Empty, "Selección inválida.");
                return RedirectToAction(nameof(Registrar), new { searchName, tipoId });
            }

            // check availability
            var disp = _context.Disponibilidades.FirstOrDefault(d => d.IdHabitacion == habitacionId && d.Fecha == fecha.Date);
            if (disp != null && !disp.EstaDisponible)
            {
                TempData["RegistrarMessage"] = "La habitación no está disponible para la fecha seleccionada.";
                return RedirectToAction(nameof(Registrar), new { searchName, tipoId });
            }

            using (var tx = await _context.Database.BeginTransactionAsync())
            {
                var reserva = new Reserva
                {
                    FechaReserva = fecha.Date,
                    Estado = "Activa",
                    IdCliente = clienteId,
                    IdHabitacion = habitacionId
                };
                _context.Reservas.Add(reserva);
                await _context.SaveChangesAsync();

                // mark disponibilidad
                if (disp == null)
                {
                    disp = new DisponibilidadHabitacion
                    {
                        IdHabitacion = habitacionId,
                        Fecha = fecha.Date,
                        EstaDisponible = false
                    };
                    _context.Disponibilidades.Add(disp);
                }
                else
                {
                    disp.EstaDisponible = false;
                    _context.Disponibilidades.Update(disp);
                }
                await _context.SaveChangesAsync();

                // add historial (use Spanish 'Creada' to match model validation)
                var historial = new HistorialReserva
                {
                    IdReserva = reserva.IdReserva,
                    FechaAccion = DateTime.Now,
                    Accion = "Creada",
                    Comentario = "Reserva registrada via Registrar"
                };
                _context.HistorialesReserva.Add(historial);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();
            }

            TempData["RegistrarMessage"] = "Reserva registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Reagendar: search client -> select active reservation -> pick new available date -> confirm
        [HttpGet]
        [HttpGet]
        [HotelmasCarga.Filters.RequireRole(HotelmasCarga.Models.Roles.AgendarReservas)]
        public IActionResult Reagendar(string searchName, int? clienteId, int? reservaId)
        {
            var vm = new Models.ReagendarReservaViewModel();
            vm.SearchName = searchName;
            vm.SelectedClienteId = clienteId;

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                vm.Clientes = _context.Clientes.Where(c => c.Nombre.Contains(searchName)).Take(20).ToList();
            }

            if (clienteId.HasValue)
            {
                vm.Reservas = _context.Reservas.Where(r => r.IdCliente == clienteId.Value && r.Estado == "Activa").Include(r => r.Habitacion).ToList();
            }

            if (reservaId.HasValue)
            {
                var reserva = _context.Reservas.Include(r => r.Habitacion).FirstOrDefault(r => r.IdReserva == reservaId.Value);
                if (reserva != null)
                {
                    vm.SelectedReserva = reserva;
                    vm.SelectedReservaId = reserva.IdReserva;

                    // next 14 days
                    var dates = new List<DateTime>();
                    for (int i = 0; i < 14; i++) dates.Add(DateTime.Today.AddDays(i));
                    vm.Dates = dates;

                    foreach (var d in dates)
                    {
                        var disp = _context.Disponibilidades.FirstOrDefault(x => x.IdHabitacion == reserva.IdHabitacion && x.Fecha == d.Date);
                        // available if not same as current reservation date and availability record is null or true
                        var available = (d.Date != reserva.FechaReserva.Date) && (disp == null || disp.EstaDisponible);
                        vm.AvailableByDate[d] = available;
                    }
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HotelmasCarga.Filters.RequireRole(HotelmasCarga.Models.Roles.AgendarReservas)]
        public async Task<IActionResult> ReagendarConfirm(int reservaId, string newDate, string searchName)
        {
            if (reservaId <= 0 || string.IsNullOrWhiteSpace(newDate))
            {
                TempData["ReagendarError"] = "Seleccione una reserva y una nueva fecha.";
                return RedirectToAction(nameof(Reagendar), new { searchName });
            }

            if (!DateTime.TryParse(newDate, out var fechaNew))
            {
                TempData["ReagendarError"] = "Fecha inválida.";
                return RedirectToAction(nameof(Reagendar), new { searchName });
            }

            var reserva = await _context.Reservas.FindAsync(reservaId);
            if (reserva == null)
            {
                TempData["ReagendarError"] = "Reserva no encontrada.";
                return RedirectToAction(nameof(Reagendar), new { searchName });
            }

            if (fechaNew.Date < DateTime.Today)
            {
                TempData["ReagendarError"] = "La nueva fecha no puede ser anterior a hoy.";
                return RedirectToAction(nameof(Reagendar), new { searchName, clienteId = reserva.IdCliente, reservaId = reservaId });
            }

            if (fechaNew.Date == reserva.FechaReserva.Date)
            {
                TempData["ReagendarError"] = "La nueva fecha no puede ser igual a la fecha actual de la reserva.";
                return RedirectToAction(nameof(Reagendar), new { searchName, clienteId = reserva.IdCliente, reservaId = reservaId });
            }

            // check availability for new date
            var dispNew = _context.Disponibilidades.FirstOrDefault(d => d.IdHabitacion == reserva.IdHabitacion && d.Fecha == fechaNew.Date);
            if (dispNew != null && !dispNew.EstaDisponible)
            {
                TempData["ReagendarError"] = "La habitación no está disponible en la nueva fecha seleccionada.";
                return RedirectToAction(nameof(Reagendar), new { searchName, clienteId = reserva.IdCliente, reservaId = reservaId });
            }

            var oldDate = reserva.FechaReserva.Date;

            using (var tx = await _context.Database.BeginTransactionAsync())
            {
                // mark previous date available
                var dispOld = _context.Disponibilidades.FirstOrDefault(d => d.IdHabitacion == reserva.IdHabitacion && d.Fecha == oldDate);
                if (dispOld == null)
                {
                    dispOld = new DisponibilidadHabitacion
                    {
                        IdHabitacion = reserva.IdHabitacion,
                        Fecha = oldDate,
                        EstaDisponible = true
                    };
                    _context.Disponibilidades.Add(dispOld);
                }
                else
                {
                    dispOld.EstaDisponible = true;
                    _context.Disponibilidades.Update(dispOld);
                }

                // mark new date unavailable
                if (dispNew == null)
                {
                    dispNew = new DisponibilidadHabitacion
                    {
                        IdHabitacion = reserva.IdHabitacion,
                        Fecha = fechaNew.Date,
                        EstaDisponible = false
                    };
                    _context.Disponibilidades.Add(dispNew);
                }
                else
                {
                    dispNew.EstaDisponible = false;
                    _context.Disponibilidades.Update(dispNew);
                }

                // update reservation
                reserva.FechaReserva = fechaNew.Date;
                _context.Reservas.Update(reserva);
                await _context.SaveChangesAsync();

                // add historial
                var historial = new HistorialReserva
                {
                    IdReserva = reserva.IdReserva,
                    FechaAccion = DateTime.Now,
                    Accion = "Reagendada",
                    Comentario = $"Reagendada desde {oldDate:yyyy-MM-dd} a {fechaNew.Date:yyyy-MM-dd}"
                };
                _context.HistorialesReserva.Add(historial);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();
            }

            TempData["ReagendarMessage"] = "Reserva reagendada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // CancelarReserva: search client -> list active reservations -> select -> confirm cancellation
        [HttpGet]
        [HotelmasCarga.Filters.RequireRole(HotelmasCarga.Models.Roles.AgendarReservas)]
        public IActionResult CancelarReserva(string searchName, int? clienteId, int? reservaId)
        {
            var vm = new Models.ReagendarReservaViewModel(); // reuse simple vm for listing
            vm.SearchName = searchName;
            vm.SelectedClienteId = clienteId;

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                vm.Clientes = _context.Clientes.Where(c => c.Nombre.Contains(searchName)).Take(20).ToList();
            }

            if (clienteId.HasValue)
            {
                vm.Reservas = _context.Reservas.Where(r => r.IdCliente == clienteId.Value && r.Estado == "Activa").Include(r => r.Habitacion).ToList();
            }

            if (reservaId.HasValue)
            {
                var reserva = _context.Reservas.Include(r => r.Habitacion).FirstOrDefault(r => r.IdReserva == reservaId.Value);
                if (reserva != null)
                {
                    vm.SelectedReserva = reserva;
                    vm.SelectedReservaId = reserva.IdReserva;
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HotelmasCarga.Filters.RequireRole(HotelmasCarga.Models.Roles.AgendarReservas)]
        public async Task<IActionResult> CancelarReservaConfirm(int reservaId, string searchName)
        {
            if (reservaId <= 0)
            {
                TempData["CancelarError"] = "Seleccione una reserva para cancelar.";
                return RedirectToAction(nameof(CancelarReserva), new { searchName });
            }

            var reserva = await _context.Reservas.FindAsync(reservaId);
            if (reserva == null)
            {
                TempData["CancelarError"] = "Reserva no encontrada.";
                return RedirectToAction(nameof(CancelarReserva), new { searchName });
            }

            if (reserva.Estado != "Activa")
            {
                TempData["CancelarError"] = "Solo se pueden cancelar reservas activas.";
                return RedirectToAction(nameof(CancelarReserva), new { searchName, clienteId = reserva.IdCliente });
            }

            var fecha = reserva.FechaReserva.Date;

            using (var tx = await _context.Database.BeginTransactionAsync())
            {
                // mark disponibilidad as available
                var disp = _context.Disponibilidades.FirstOrDefault(d => d.IdHabitacion == reserva.IdHabitacion && d.Fecha == fecha);
                if (disp == null)
                {
                    disp = new DisponibilidadHabitacion
                    {
                        IdHabitacion = reserva.IdHabitacion,
                        Fecha = fecha,
                        EstaDisponible = true
                    };
                    _context.Disponibilidades.Add(disp);
                }
                else
                {
                    disp.EstaDisponible = true;
                    _context.Disponibilidades.Update(disp);
                }

                // update reserva status
                reserva.Estado = "Cancelada";
                _context.Reservas.Update(reserva);
                await _context.SaveChangesAsync();

                // add historial
                var historial = new HistorialReserva
                {
                    IdReserva = reserva.IdReserva,
                    FechaAccion = DateTime.Now,
                    Accion = "Cancelada",
                    Comentario = "Reserva cancelada via CancelarReserva"
                };
                _context.HistorialesReserva.Add(historial);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();
            }

            TempData["CancelarMessage"] = "Reserva cancelada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var reserva = await _context.Reservas.Include(r => r.Cliente).Include(r => r.Habitacion).FirstOrDefaultAsync(r => r.IdReserva == id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
