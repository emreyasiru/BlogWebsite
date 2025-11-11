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
        [HttpGet]
        public IActionResult Kategoriler()
        {
            var kategoriler = _db.Kategorilers.ToList();
            var userId = HttpContext.Session.GetInt32("KullaniciId");
            if (userId == null)
            {
                return RedirectToAction("LoginPage");
            }

            return View(kategoriler);
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
            return RedirectToAction("Kategoriler");
        }
        [HttpGet]
        public IActionResult Etiketler()
        {
            var etiketler = _db.Etiketlers.ToList();
            var userId = HttpContext.Session.GetInt32("KullaniciId");
            if (userId == null)
            {
                return RedirectToAction("LoginPage");
            }

            return View(etiketler);
        }
        [HttpPost]
        public IActionResult EtiketEkle(string Etiket_ekle)
        {
            var etkekle = new Etiketler
            {
                EtiketAdi = Etiket_ekle,

            };

            _db.Etiketlers.Add(etkekle);
            _db.SaveChanges();
            return RedirectToAction("Etiketler");
        }
        [HttpGet]
        public IActionResult BlogEkle()
        {
            var userId = HttpContext.Session.GetInt32("KullaniciId");
            if (userId == null)
            {
                return RedirectToAction("LoginPage");
            }

            return View();
        }


    }
}