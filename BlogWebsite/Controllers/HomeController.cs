using System.Diagnostics;
using BlogWebsite.Modeller;
using BlogWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BlogDbContext _db;

        public HomeController(ILogger<HomeController> logger, BlogDbContext db)
        {
            _db = db;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var blogs = new AnasayfaBlogs();
            {
                blogs.BlogYazilarim = _db.BlogYazilaris.ToList();
                blogs.Etiketlerim = _db.Etiketlers.ToList();
                blogs.Kategorilerim = _db.Kategorilers.ToList();
                blogs.Yorumlarim = _db.Yorumlars.ToList();
                blogs.Kullanicilarim = _db.Kullanicilars.ToList();
                blogs.BlogEtiketlerim = _db.BlogEtiketlers.ToList();


            }
            return View(blogs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
