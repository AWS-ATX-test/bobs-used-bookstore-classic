using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Bookstore.Web.Areas.Admin.Controllers
{
[Route("Admin")]
[Authorize(Roles = "Administrators")]
public abstract class AdminAreaControllerBase : Controller { }
}
