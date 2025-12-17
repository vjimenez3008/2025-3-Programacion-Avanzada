using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelmasCarga.Data;
using HotelmasCarga.Models;

namespace HotelmasCarga.Controllers
{
    [HotelmasCarga.Filters.RequireRole(HotelmasCarga.Models.Roles.AgendarReservas)]
    public class ColaEsperaController : Controller
    {
        private readonly HotelmasCargaContext _context;
        public ColaEsperaController(HotelmasCargaContext context) => _context = context;

        // List waiting entries with optional filters
        public IActionResult Index(int? tipoId, DateTime? fechaDeseada)
        {
            ViewData["Tipos"] = new SelectList(_context.TiposHabitacion, "IdTipoHabitacion", "Nombre", tipoId);
            var q = _context.ColasEspera.Include(c => c.Cliente).Include(c => c.TipoHabitacion).AsQueryable();
            if (tipoId.HasValue) q = q.Where(c => c.IdTipoHabitacion == tipoId.Value);
            if (fechaDeseada.HasValue) q = q.Where(c => c.FechaDeseada == fechaDeseada.Value.Date);

            var list = q.OrderBy(c => c.FechaRegistro).ToList();

            // compute status per requirement (do not persist automatic changes)
            ViewData["FechaFiltro"] = fechaDeseada?.ToString("yyyy-MM-dd") ?? string.Empty;

            return View(list);
        }

        // Show form to add to waiting list
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Clientes"] = new SelectList(_context.Clientes.Where(c => c.Activo), "IdCliente", "Nombre");
            ViewData["Tipos"] = new SelectList(_context.TiposHabitacion, "IdTipoHabitacion", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCliente,IdTipoHabitacion,FechaDeseada")] ColaEspera cola)
        {
            if (cola == null)
            {
                ModelState.AddModelError(string.Empty, "Datos inválidos.");
                return RedirectToAction(nameof(Create));
            }

            // server-side validation
            if (cola.IdCliente <= 0) ModelState.AddModelError("IdCliente", "Seleccione un cliente.");
            if (cola.IdTipoHabitacion <= 0) ModelState.AddModelError("IdTipoHabitacion", "Seleccione un tipo de habitación.");
            if (cola.FechaDeseada == default) ModelState.AddModelError("FechaDeseada", "Ingrese una fecha deseada.");

            if (!ModelState.IsValid)
            {
                ViewData["Clientes"] = new SelectList(_context.Clientes.Where(c => c.Activo), "IdCliente", "Nombre", cola.IdCliente);
                ViewData["Tipos"] = new SelectList(_context.TiposHabitacion, "IdTipoHabitacion", "Nombre", cola.IdTipoHabitacion);
                return View(cola);
            }

            cola.FechaRegistro = DateTime.Now;
            cola.Estado = "Activo";

            _context.ColasEspera.Add(cola);
            await _context.SaveChangesAsync();

            TempData["ColaMessage"] = "Ingresado en la lista de espera correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.ColasEspera.Include(c => c.Cliente).Include(c => c.TipoHabitacion).FirstOrDefaultAsync(c => c.IdCola == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = _context.ColasEspera.Find(id);
            if (item == null) return NotFound();
            ViewData["Clientes"] = new SelectList(_context.Clientes.Where(c => c.Activo), "IdCliente", "Nombre", item.IdCliente);
            ViewData["Tipos"] = new SelectList(_context.TiposHabitacion, "IdTipoHabitacion", "Nombre", item.IdTipoHabitacion);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCola,IdCliente,IdTipoHabitacion,FechaDeseada,FechaRegistro,Estado")] ColaEspera cola)
        {
            if (id != cola.IdCola) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewData["Clientes"] = new SelectList(_context.Clientes.Where(c => c.Activo), "IdCliente", "Nombre", cola.IdCliente);
                ViewData["Tipos"] = new SelectList(_context.TiposHabitacion, "IdTipoHabitacion", "Nombre", cola.IdTipoHabitacion);
                return View(cola);
            }

            try
            {
                _context.Update(cola);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ColasEspera.Any(e => e.IdCola == cola.IdCola)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.ColasEspera.Include(c => c.Cliente).Include(c => c.TipoHabitacion).FirstOrDefaultAsync(c => c.IdCola == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.ColasEspera.FindAsync(id);
            if (item != null)
            {
                _context.ColasEspera.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
