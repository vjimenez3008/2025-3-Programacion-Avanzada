using Microsoft.AspNetCore.Mvc;
using HotelmasCarga.Helpers;
using HotelmasCarga.Models;

namespace HotelmasCarga.Controllers
{
    public class RoleController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Roles"] = new[] { Roles.AgendarReservas, Roles.Mantenimientos };
            ViewData["Active"] = RoleHelper.GetActiveRole(HttpContext) ?? string.Empty;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Set(string role)
        {
            if (role == Roles.AgendarReservas || role == Roles.Mantenimientos)
            {
                RoleHelper.SetActiveRole(HttpContext, role);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
