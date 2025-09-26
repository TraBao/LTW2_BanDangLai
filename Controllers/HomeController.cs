using Microsoft.AspNetCore.Mvc;

namespace WebAPI_template.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
