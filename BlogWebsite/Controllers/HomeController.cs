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

        [HttpGet]
        public IActionResult BlogDetay(int? blogid)
        {
            if (blogid == null || blogid == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var bloglarým = _db.BlogYazilaris.FirstOrDefault(x => x.BlogId == blogid);
            var yorumlarým = _db.Yorumlars.Where(x => x.BlogId == blogid).ToList();
            var kategori = _db.Kategorilers.Where(x => x.KategoriId == bloglarým.KategoriId).FirstOrDefault();
            var etiketler = (from be in _db.BlogEtiketlers
                             join e in _db.Etiketlers on be.EtiketId equals e.EtiketId
                             where be.BlogId == blogid
                             select e).ToList();
            var kullanici = _db.Kullanicilars.Where(x => x.KullaniciId == bloglarým.YazarId).FirstOrDefault();
            ViewBag.YorumSayisi = yorumlarým.Count(x => x.Onaylandi == true);
            var model = new AnasayfaBlogs();
            {
                model.BlogYazilarim = new List<BlogYazilari> { bloglarým };
                model.Yorumlarim = yorumlarým;
                model.Kategorilerim = new List<Kategoriler> { kategori };
                model.Etiketlerim = etiketler;
                model.Kullanicilarim = new List<Kullanicilar> { kullanici };
            }
            return View(model);
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
