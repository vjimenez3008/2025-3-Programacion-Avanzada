using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelmasCarga.Data;
using HotelmasCarga.Models;

namespace HotelmasCarga.Controllers
{
    [HotelmasCarga.Filters.RequireRole(HotelmasCarga.Models.Roles.Mantenimientos)]
    public class TipoHabitacionController : Controller
    {
        private readonly HotelmasCargaContext _context;

        public TipoHabitacionController(HotelmasCargaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.TiposHabitacion.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var tipo = await _context.TiposHabitacion.FirstOrDefaultAsync(t => t.IdTipoHabitacion == id);
            if (tipo == null) return NotFound();
            return View(tipo);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Precio")] TipoHabitacion tipoHabitacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoHabitacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoHabitacion);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var tipo = await _context.TiposHabitacion.FindAsync(id);
            if (tipo == null) return NotFound();
            return View(tipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoHabitacion,Nombre,Descripcion,Precio")] TipoHabitacion tipoHabitacion)
        {
            if (id != tipoHabitacion.IdTipoHabitacion) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(tipoHabitacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoHabitacion);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var tipo = await _context.TiposHabitacion.FirstOrDefaultAsync(t => t.IdTipoHabitacion == id);
            if (tipo == null) return NotFound();
            return View(tipo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipo = await _context.TiposHabitacion.FindAsync(id);
            if (tipo != null)
            {
                _context.TiposHabitacion.Remove(tipo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
