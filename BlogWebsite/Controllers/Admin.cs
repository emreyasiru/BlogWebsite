using BlogWebsite.Modeller;
using BlogWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebsite.Controllers
{
    public class Admin : Controller
    {

        private readonly BlogDbContext _db;
        private readonly IWebHostEnvironment _env;

        public Admin(BlogDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
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

            var model = new KategoriVeEtiket
            {
                kategorilerim = _db.Kategorilers.ToList(),
                etiketlerim = _db.Etiketlers.OrderBy(e => e.EtiketAdi).ToList()
            };

            return View(model);
        }
        // Blog kaydetme
        [HttpPost]
        public async Task<IActionResult> BlogEkle(

            string Baslik,
            string Ozet,
            string Icerik,
            int KategoriId,
            IFormFile AnaGorselDosya,
            List<int> SecilenEtiketler)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("KullaniciId");

                if (userId == null)
                {
                    return RedirectToAction("LoginPage");
                }
                // Validasyon
                if (string.IsNullOrWhiteSpace(Baslik))
                {
                    TempData["Hata"] = "Başlık zorunludur!";
                    return RedirectToAction("BlogEkle");
                }

                if (string.IsNullOrWhiteSpace(Icerik))
                {
                    TempData["Hata"] = "İçerik zorunludur!";
                    return RedirectToAction("BlogEkle");
                }

                var blog = new BlogYazilari
                {
                    Baslik = Baslik,
                    Ozet = Ozet,
                    Icerik = Icerik,
                    KategoriId = KategoriId,
                    YazarId = userId.Value, // Session'dan alınan değer
                    GoruntulemeSayisi = 0,

                };

                // Resim yükleme
                if (AnaGorselDosya != null && AnaGorselDosya.Length > 0)
                {
                    string uploadKlasoru = Path.Combine(_env.WebRootPath, "uploads", "blog");

                    if (!Directory.Exists(uploadKlasoru))
                    {
                        Directory.CreateDirectory(uploadKlasoru);
                    }

                    string dosyaUzantisi = Path.GetExtension(AnaGorselDosya.FileName);
                    string yeniDosyaAdi = $"{Guid.NewGuid()}{dosyaUzantisi}";
                    string dosyaYolu = Path.Combine(uploadKlasoru, yeniDosyaAdi);

                    using (var stream = new FileStream(dosyaYolu, FileMode.Create))
                    {
                        await AnaGorselDosya.CopyToAsync(stream);
                    }

                    blog.AnaGorsel = $"/uploads/blog/{yeniDosyaAdi}";
                }

                // Blog'u kaydet
                _db.BlogYazilaris.Add(blog);
                await _db.SaveChangesAsync();

                // Etiketleri kaydet
                if (SecilenEtiketler != null && SecilenEtiketler.Any())
                {
                    foreach (var etiketId in SecilenEtiketler)
                    {
                        _db.BlogEtiketlers.Add(new BlogEtiketler
                        {
                            BlogId = blog.BlogId,
                            EtiketId = etiketId,

                        });

                        // Etiket kullanım sayısını artır
                        var etiket = await _db.Etiketlers.FindAsync(etiketId);
                        if (etiket != null)
                        {
                            etiket.KullanimSayisi++;
                        }
                    }

                    await _db.SaveChangesAsync();
                }

                TempData["Basarili"] = "Blog yazınız başarıyla kaydedildi ve admin onayı bekliyor.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Hata"] = $"Bir hata oluştu: {ex.Message}";
                return RedirectToAction("BlogEkle");
            }
        }
        // Blog onay listesini göster
        [HttpGet]
        public IActionResult BlogOnay()
        {
            // Session'dan kullanıcı ID'sini al
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            if (kullaniciId == null)
            {
                return RedirectToAction("LoginPage", "Admin");
            }

            // Kullanıcının admin olup olmadığını kontrol et
            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == kullaniciId);

            if (kullanici == null || kullanici.Rol != "Admin")
            {
                return RedirectToAction("LoginPage", "Admin");
            }

            // Durum = 0 olan (onay bekleyen) blogları getir
            var onayBekleyenBloglar = _db.BlogYazilaris
                .Include(b => b.Yazar)
                .Include(b => b.Kategori)
                .Where(b => b.Durum == 0)
                .OrderByDescending(b => b.YayinTarihi)
                .ToList();

            return View(onayBekleyenBloglar);
        }
        // Blog onaylama işlemi
        [HttpPost]
        public IActionResult BlogOnayla(int blogId)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            if (kullaniciId == null)
            {
                return Json(new { success = false, message = "Oturum bulunamadı!" });
            }

            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == kullaniciId);

            if (kullanici == null || kullanici.Rol != "Admin")
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var blog = _db.BlogYazilaris.FirstOrDefault(b => b.BlogId == blogId);

            if (blog == null)
            {
                return Json(new { success = false, message = "Blog bulunamadı!" });
            }

            blog.Durum = 1;
            blog.YayinTarihi = DateTime.Now;
            _db.SaveChanges();

            return Json(new { success = true, message = "Blog onaylandı!" });
        }

        [HttpPost]
        public IActionResult BlogReddet(int blogId)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            if (kullaniciId == null)
            {
                return Json(new { success = false, message = "Oturum bulunamadı!" });
            }

            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == kullaniciId);

            if (kullanici == null || kullanici.Rol != "Admin")
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var blog = _db.BlogYazilaris.FirstOrDefault(b => b.BlogId == blogId);

            if (blog == null)
            {
                return Json(new { success = false, message = "Blog bulunamadı!" });
            }

            // Durum = 2 (Reddedildi) yap, silme
            blog.Durum = 2;
            _db.SaveChanges();

            return Json(new { success = true, message = "Blog reddedildi!" });
        }
        [HttpGet]
        public IActionResult YorumOnay()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            if (kullaniciId == null)
            {
                return RedirectToAction("LoginPage", "Admin");
            }

            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == kullaniciId);

            if (kullanici == null || kullanici.Rol != "Admin")
            {
                return RedirectToAction("LoginPage", "Admin");
            }

            // Onay bekleyen yorumları getir (Onaylandı = false)
            var onayBekleyenYorumlar = _db.Yorumlars
                .Include(y => y.Blog)
                .Where(y => y.Onaylandi == false)
                .OrderByDescending(y => y.OlusturmaTarihi)
                .ToList();

            return View(onayBekleyenYorumlar);
        }

        // Yorum onaylama
        [HttpPost]
        public IActionResult YorumOnayla(int yorumId)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            if (kullaniciId == null)
            {
                return Json(new { success = false, message = "Oturum bulunamadı!" });
            }

            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == kullaniciId);

            if (kullanici == null || kullanici.Rol != "Admin")
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var yorum = _db.Yorumlars.FirstOrDefault(y => y.YorumId == yorumId);

            if (yorum == null)
            {
                return Json(new { success = false, message = "Yorum bulunamadı!" });
            }

            yorum.Onaylandi = true;
            _db.SaveChanges();

            return Json(new { success = true, message = "Yorum onaylandı!" });
        }

        // Yorum reddetme
        [HttpPost]
        public IActionResult YorumReddet(int yorumId)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            if (kullaniciId == null)
            {
                return Json(new { success = false, message = "Oturum bulunamadı!" });
            }

            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == kullaniciId);

            if (kullanici == null || kullanici.Rol != "Admin")
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var yorum = _db.Yorumlars.FirstOrDefault(y => y.YorumId == yorumId);

            if (yorum == null)
            {
                return Json(new { success = false, message = "Yorum bulunamadı!" });
            }

            // Yorumu sil
            _db.Yorumlars.Remove(yorum);
            _db.SaveChanges();

            return Json(new { success = true, message = "Yorum reddedildi ve silindi!" });
        }




    }
}