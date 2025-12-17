using Microsoft.AspNetCore.Http;
using HotelmasCarga.Models;

namespace HotelmasCarga.Helpers
{
    public static class RoleHelper
    {
        private const string SessionKey = "ActiveRole";

        public static void SetActiveRole(HttpContext http, string role)
        {
            http.Session.SetString(SessionKey, role ?? string.Empty);
        }

        public static string? GetActiveRole(HttpContext http)
        {
            return http.Session.GetString(SessionKey);
        }
    }
}
