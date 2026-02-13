using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;


namespace Bookstore.Web.Areas
{
    public static class AdminAreaRegistration
    {
        public static void Register(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "Admin_default",
                pattern: "Admin/{controller}/{action}/{id}",
                defaults: new { action = "Index", id = "" },
                constraints: null,
                dataTokens: new { area = "Admin", namespace_ = "Bookstore.Web.Areas.Admin.Controllers" }
            );
        }
    }
}
