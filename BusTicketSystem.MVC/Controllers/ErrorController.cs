using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode:int}")]
        public IActionResult Index(int statusCode)
        {
            Response.StatusCode = statusCode;

            if (statusCode == StatusCodes.Status404NotFound)
            {
                return View("NotFound");
            }

            return View("ServerError");
        }

        [Route("Error/500")]
        public IActionResult ServerError()
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ViewData["ErrorPath"] = exceptionFeature?.Path;
            return View();
        }
    }
}
