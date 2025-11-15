using System.Diagnostics;
using BlogWebsite.Modeller;
using BlogWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            // Kategorileri ve her kategorideki blog sayýsýný getir
            var kategoriler = _db.Kategorilers.ToList();
            foreach (var kategori in kategoriler)
            {
                kategori.BlogYazilaris = _db.BlogYazilaris
                    .Where(b => b.KategoriId == kategori.KategoriId && b.Durum == 1)
                    .ToList();
            }

            var blogs = new AnasayfaBlogs();
            {
                blogs.BlogYazilarim = _db.BlogYazilaris.Where(b => b.Durum == 1).ToList(); // Sadece onaylý bloglar
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

        [HttpPost]
        public IActionResult YorumYap(string isim, string email, string yorum, int blogId)
        {
            if (string.IsNullOrWhiteSpace(isim) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(yorum))
            {
                TempData["Hata"] = "Tüm alanlarý doldurun!";
                return RedirectToAction("BlogDetay", "Home", new { blogid = blogId });
            }

            var blog = _db.BlogYazilaris.FirstOrDefault(b => b.BlogId == blogId);

            if (blog == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var yorumekle = new Yorumlar
            {
                BlogId = blogId,
                Ad = isim,
                Email = email,
                YorumIcerik = yorum,
                Onaylandi = false,
                OlusturmaTarihi = DateTime.Now
            };

            _db.Yorumlars.Add(yorumekle);
            _db.SaveChanges();

            TempData["YorumBasari"] = "Yorumunuz baþarýyla gönderildi! Onaylandýktan sonra yayýnlanacaktýr.";
            return RedirectToAction("BlogDetay", "Home", new { blogid = blogId });
        }
        [HttpGet]
        public IActionResult KategoriBloglari(int? kategoriId)
        {
            if (kategoriId == null || kategoriId == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // Kategoriyi getir
            var kategori = _db.Kategorilers.FirstOrDefault(k => k.KategoriId == kategoriId);

            if (kategori == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Bu kategoriye ait onaylý bloglarý getir
            var kategoriBloglar = _db.BlogYazilaris
                .Include(b => b.Yazar)
                .Include(b => b.Kategori)
                .Where(b => b.KategoriId == kategoriId && b.Durum == 1)
                .OrderByDescending(b => b.YayinTarihi)
                .ToList();

            var yorumlar = _db.Yorumlars.Where(y => y.Onaylandi == true).ToList();
            var etiketler = _db.Etiketlers.ToList();
            var blogetiketler = _db.BlogEtiketlers.ToList();
            var kullanicilar = _db.Kullanicilars.ToList();

            // Sidebar için tüm kategorileri ve blog sayýlarýný getir
            var tumKategoriler = _db.Kategorilers.ToList();
            foreach (var kat in tumKategoriler)
            {
                kat.BlogYazilaris = _db.BlogYazilaris
                    .Where(b => b.KategoriId == kat.KategoriId && b.Durum == 1)
                    .ToList();
            }

            var model = new AnasayfaBlogs
            {
                BlogYazilarim = kategoriBloglar,
                Yorumlarim = yorumlar,
                Etiketlerim = etiketler,
                BlogEtiketlerim = blogetiketler,
                Kullanicilarim = kullanicilar,
                Kategorilerim = tumKategoriler
            };

            // Seçili kategori bilgisini ViewBag ile gönder
            ViewBag.SecilikKategori = kategori;

            return View(model);
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
