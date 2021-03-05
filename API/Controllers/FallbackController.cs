using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FallbackController : Controller
    {
        // angular app is the view for application
        // controller class will handle the view for the angular app
        public ActionResult Index()
        {
            // if api doesn't know what to do with route it falls back to this controller
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
        }
    }
}