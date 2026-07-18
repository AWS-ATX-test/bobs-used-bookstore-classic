using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Web.Areas
{
    // [PORT-TODO: ASP.NET Core does not have AreaRegistration. Move this route to Program.cs as:
    //   app.MapControllerRoute(name: "Admin_default", pattern: "Admin/{controller=Home}/{action=Index}/{id?}");
    // Ensure Admin controllers are decorated with [Area("Admin")]. Then delete this class.]
    public static class AdminAreaRegistration
    {
        public static string AreaName => "Admin";

        public static void RegisterAdminAreaRoute(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "Admin_default",
                pattern: "Admin/{controller}/{action}/{id?}",
                defaults: new { action = "Index" },
                constraints: null,
                dataTokens: new { area = "Admin" });
        }
    }
}
