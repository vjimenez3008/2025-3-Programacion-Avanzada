using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelmasCarga.Data;
using HotelmasCarga.Models;

namespace HotelmasCarga.Controllers
{
    [HotelmasCarga.Filters.RequireRole(HotelmasCarga.Models.Roles.Mantenimientos)]
    public class HabitacionController : Controller
    {
        private readonly HotelmasCargaContext _context;
        public HabitacionController(HotelmasCargaContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var habitaciones = _context.Habitaciones.Include(h => h.TipoHabitacion);
            return View(await habitaciones.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var habitacion = await _context.Habitaciones.Include(h => h.TipoHabitacion).FirstOrDefaultAsync(h => h.IdHabitacion == id);
            if (habitacion == null) return NotFound();
            return View(habitacion);
        }

        public IActionResult Create()
        {
            ViewData["Tipos"] = _context.TiposHabitacion.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Numero,Activa,IdTipoHabitacion")] Habitacion habitacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(habitacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Tipos"] = _context.TiposHabitacion.ToList();
            return View(habitacion);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null) return NotFound();
            ViewData["Tipos"] = _context.TiposHabitacion.ToList();
            return View(habitacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdHabitacion,Numero,Activa,IdTipoHabitacion")] Habitacion habitacion)
        {
            if (id != habitacion.IdHabitacion) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(habitacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Tipos"] = _context.TiposHabitacion.ToList();
            return View(habitacion);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var habitacion = await _context.Habitaciones.Include(h => h.TipoHabitacion).FirstOrDefaultAsync(h => h.IdHabitacion == id);
            if (habitacion == null) return NotFound();
            return View(habitacion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion != null)
            {
                _context.Habitaciones.Remove(habitacion);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
