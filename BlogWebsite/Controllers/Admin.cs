using Microsoft.AspNetCore.Mvc;

namespace BlogWebsite.Controllers
{
    public class Admin : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
