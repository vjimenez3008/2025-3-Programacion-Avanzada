using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelmasCarga.Data;
using HotelmasCarga.Models;

namespace HotelmasCarga.Controllers
{
    public class DisponibilidadHabitacionController : Controller
    {
        private readonly HotelmasCargaContext _context;
        public DisponibilidadHabitacionController(HotelmasCargaContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var list = _context.Disponibilidades.Include(d => d.Habitacion).ThenInclude(h => h!.TipoHabitacion);
            return View(await list.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Disponibilidades.Include(d => d.Habitacion).ThenInclude(h => h!.TipoHabitacion)
                .FirstOrDefaultAsync(d => d.IdDisponibilidad == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create()
        {
            ViewData["Habitaciones"] = new SelectList(_context.Habitaciones.Include(h => h.TipoHabitacion).ToList(), "IdHabitacion", "Numero");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Fecha,EstaDisponible,IdHabitacion")] DisponibilidadHabitacion disponibilidad)
        {
            if (disponibilidad == null)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                _context.Add(disponibilidad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Habitaciones"] = new SelectList(_context.Habitaciones.Include(h => h.TipoHabitacion).ToList(), "IdHabitacion", "Numero", disponibilidad.IdHabitacion);
            return View(disponibilidad);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Disponibilidades.FindAsync(id);
            if (item == null) return NotFound();
            ViewData["Habitaciones"] = new SelectList(_context.Habitaciones.Include(h => h.TipoHabitacion).ToList(), "IdHabitacion", "Numero", item.IdHabitacion);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDisponibilidad,Fecha,EstaDisponible,IdHabitacion")] DisponibilidadHabitacion disponibilidad)
        {
            if (id != disponibilidad.IdDisponibilidad) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(disponibilidad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Disponibilidades.Any(e => e.IdDisponibilidad == disponibilidad.IdDisponibilidad)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Habitaciones"] = new SelectList(_context.Habitaciones.Include(h => h.TipoHabitacion).ToList(), "IdHabitacion", "Numero", disponibilidad.IdHabitacion);
            return View(disponibilidad);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Disponibilidades.Include(d => d.Habitacion).ThenInclude(h => h!.TipoHabitacion).FirstOrDefaultAsync(d => d.IdDisponibilidad == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Disponibilidades.FindAsync(id);
            if (item != null)
            {
                _context.Disponibilidades.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
