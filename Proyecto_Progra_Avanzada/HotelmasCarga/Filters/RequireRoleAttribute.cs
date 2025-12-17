using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using HotelmasCarga.Helpers;

namespace HotelmasCarga.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequireRoleAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _role;

        public RequireRoleAttribute(string role)
        {
            _role = role;
        }

        public async System.Threading.Tasks.Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var http = context.HttpContext;
            var active = RoleHelper.GetActiveRole(http);
            if (string.IsNullOrEmpty(active) || active != _role)
            {
                // redirect to home with message
                context.HttpContext.Response.Redirect("/Home/Index");
                return;
            }
            await next();
        }
    }
}
