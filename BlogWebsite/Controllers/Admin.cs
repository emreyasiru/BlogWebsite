using BlogWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebsite.Controllers
{
    public class Admin : Controller
    {
        private readonly BlogDbContext _db;

        public Admin(BlogDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public IActionResult LoginPage()
        {
            // Zaten giriş yapmışsa Index'e yönlendir
            var userId = HttpContext.Session.GetInt32("KullaniciId");
            if (userId != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        // POST: Login işlemi
        [HttpPost]
        public IActionResult LoginPage(string kullaniciAdi, string sifre)
        {
            var kullanici = _db.Kullanicilars
                .FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi && x.Sifre == sifre);

            if (kullanici != null)
            {
                HttpContext.Session.SetInt32("KullaniciId", kullanici.KullaniciId);
                HttpContext.Session.SetString("KullaniciAdi", kullanici.KullaniciAdi);
                HttpContext.Session.SetString("Rol", kullanici.Rol);
                return RedirectToAction("Index");
            }

            ViewBag.Hata = "Kullanıcı adı veya şifre yanlış.";
            return View();
        }
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("KullaniciId");
            if (userId == null)
            {
                return RedirectToAction("LoginPage");
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("KullaniciAdi");
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginPage");
        }
        public IActionResult Kategoriler()
        {
            var userId = HttpContext.Session.GetInt32("KullaniciId");
            if (userId == null)
            {
                return RedirectToAction("LoginPage");
            }
            return View();
        }
        [HttpPost]
        public IActionResult KategoriEkle(string kategori_ekle)
        {
            var ktgekle = new Kategoriler
            {
                KategoriAdi = kategori_ekle
            };

            _db.Kategorilers.Add(ktgekle);
            _db.SaveChanges();
            return View("Kategoriler");
        }

    }
}