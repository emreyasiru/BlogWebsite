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
        [HttpGet]
        public IActionResult Index()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            if (kullaniciId == null)
            {
                return RedirectToAction("LoginPage");
            }

            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == kullaniciId);

            if (kullanici == null || kullanici.Rol != "Admin")
            {
                return RedirectToAction("BlogEkle", "Admin");
            }

            // Durum = 1 olan (yayınlanmış) blogları getir
            var yayinlanmisBloglar = _db.BlogYazilaris
                .Include(b => b.Yazar)
                .Include(b => b.Kategori)
                .Where(b => b.Durum == 1)
                .OrderByDescending(b => b.YayinTarihi)
                .ToList();

            return View(yayinlanmisBloglar);
        }

        // Blog pasifleştirme (Durum = 2 yap)
        [HttpPost]
        public IActionResult BlogPasiflestir(int blogId)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == kullaniciId);

            // Sadece Admin pasifleştirebilir
            if (kullanici == null || kullanici.Rol != "Admin")
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var blog = _db.BlogYazilaris.FirstOrDefault(b => b.BlogId == blogId);

            if (blog == null)
            {
                return Json(new { success = false, message = "Blog bulunamadı!" });
            }

            // Durum = 2 (Pasif/Yayından kaldırıldı)
            blog.Durum = 2;
            _db.SaveChanges();

            return Json(new { success = true, message = "Blog yayından kaldırıldı!" });
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginPage");
        }
        [HttpGet]
        public IActionResult Kategoriler()
        {
            var userId = HttpContext.Session.GetInt32("KullaniciId");
            if (userId == null)
            {
                return RedirectToAction("LoginPage");
            }

            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == userId);
            if (kullanici == null)
            {
                return RedirectToAction("LoginPage");
            }

            // Kullanıcı rolünü ViewBag ile gönder
            ViewBag.KullaniciRol = kullanici.Rol;

            var kategoriler = _db.Kategorilers.ToList();
            return View(kategoriler);
        }

        [HttpPost]
        public IActionResult KategoriEkle(string kategori_ekle)
        {
            var userId = HttpContext.Session.GetInt32("KullaniciId");
            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == userId);

            // Sadece Admin ekleyebilir
            if (kullanici == null || kullanici.Rol != "Admin")
            {
                TempData["Hata"] = "Yetkiniz yok!";
                return RedirectToAction("Kategoriler");
            }

            if (string.IsNullOrWhiteSpace(kategori_ekle))
            {
                TempData["Hata"] = "Kategori adı boş olamaz!";
                return RedirectToAction("Kategoriler");
            }

            var ktgekle = new Kategoriler
            {
                KategoriAdi = kategori_ekle
            };
            _db.Kategorilers.Add(ktgekle);
            _db.SaveChanges();

            TempData["Basari"] = "Kategori başarıyla eklendi!";
            return RedirectToAction("Kategoriler");
        }

        [HttpPost]
        public IActionResult KategoriGuncelle(int kategoriId, string kategoriAdi)
        {
            var userId = HttpContext.Session.GetInt32("KullaniciId");
            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == userId);

            // Sadece Admin güncelleyebilir
            if (kullanici == null || kullanici.Rol != "Admin")
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            if (string.IsNullOrWhiteSpace(kategoriAdi))
            {
                return Json(new { success = false, message = "Kategori adı boş olamaz!" });
            }

            var kategori = _db.Kategorilers.FirstOrDefault(k => k.KategoriId == kategoriId);

            if (kategori == null)
            {
                return Json(new { success = false, message = "Kategori bulunamadı!" });
            }

            kategori.KategoriAdi = kategoriAdi;
            _db.SaveChanges();

            return Json(new { success = true, message = "Kategori güncellendi!" });
        }

        [HttpPost]
        public IActionResult KategoriSil(int kategoriId)
        {
            var userId = HttpContext.Session.GetInt32("KullaniciId");
            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == userId);

            // Sadece Admin silebilir
            if (kullanici == null || kullanici.Rol != "Admin")
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var kategori = _db.Kategorilers.FirstOrDefault(k => k.KategoriId == kategoriId);

            if (kategori == null)
            {
                return Json(new { success = false, message = "Kategori bulunamadı!" });
            }

            // Kategoriye ait blog var mı kontrol et
            var kategoriBlogSayisi = _db.BlogYazilaris.Count(b => b.KategoriId == kategoriId);

            if (kategoriBlogSayisi > 0)
            {
                return Json(new { success = false, message = $"Bu kategoriye ait {kategoriBlogSayisi} blog var! Önce blogları silmelisiniz." });
            }

            _db.Kategorilers.Remove(kategori);
            _db.SaveChanges();

            return Json(new { success = true, message = "Kategori silindi!" });
        }
        [HttpGet]
        public IActionResult Etiketler()
        {
            var userId = HttpContext.Session.GetInt32("KullaniciId");
            if (userId == null)
            {
                return RedirectToAction("LoginPage");
            }

            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == userId);
            if (kullanici == null)
            {
                return RedirectToAction("LoginPage");
            }

            // Kullanıcı rolünü ViewBag ile gönder
            ViewBag.KullaniciRol = kullanici.Rol;

            var etiketler = _db.Etiketlers.ToList();
            return View(etiketler);
        }

        [HttpPost]
        public IActionResult EtiketEkle(string Etiket_ekle)
        {
            var userId = HttpContext.Session.GetInt32("KullaniciId");
            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == userId);

            // Sadece Admin ekleyebilir
            if (kullanici == null || kullanici.Rol != "Admin")
            {
                TempData["Hata"] = "Yetkiniz yok!";
                return RedirectToAction("Etiketler");
            }

            if (string.IsNullOrWhiteSpace(Etiket_ekle))
            {
                TempData["Hata"] = "Etiket adı boş olamaz!";
                return RedirectToAction("Etiketler");
            }

            var etkekle = new Etiketler
            {
                EtiketAdi = Etiket_ekle,
                KullanimSayisi = 0
            };
            _db.Etiketlers.Add(etkekle);
            _db.SaveChanges();

            TempData["Basari"] = "Etiket başarıyla eklendi!";
            return RedirectToAction("Etiketler");
        }

        [HttpPost]
        public IActionResult EtiketGuncelle(int etiketId, string etiketAdi)
        {
            var userId = HttpContext.Session.GetInt32("KullaniciId");
            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == userId);

            // Sadece Admin güncelleyebilir
            if (kullanici == null || kullanici.Rol != "Admin")
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            if (string.IsNullOrWhiteSpace(etiketAdi))
            {
                return Json(new { success = false, message = "Etiket adı boş olamaz!" });
            }

            var etiket = _db.Etiketlers.FirstOrDefault(e => e.EtiketId == etiketId);

            if (etiket == null)
            {
                return Json(new { success = false, message = "Etiket bulunamadı!" });
            }

            etiket.EtiketAdi = etiketAdi;
            _db.SaveChanges();

            return Json(new { success = true, message = "Etiket güncellendi!" });
        }

        [HttpPost]
        public IActionResult EtiketSil(int etiketId)
        {
            var userId = HttpContext.Session.GetInt32("KullaniciId");
            var kullanici = _db.Kullanicilars.FirstOrDefault(k => k.KullaniciId == userId);

            // Sadece Admin silebilir
            if (kullanici == null || kullanici.Rol != "Admin")
            {
                return Json(new { success = false, message = "Yetkiniz yok!" });
            }

            var etiket = _db.Etiketlers.FirstOrDefault(e => e.EtiketId == etiketId);

            if (etiket == null)
            {
                return Json(new { success = false, message = "Etiket bulunamadı!" });
            }

            // Etikete ait blog var mı kontrol et
            var etiketBlogSayisi = _db.BlogEtiketlers.Count(be => be.EtiketId == etiketId);

            if (etiketBlogSayisi > 0)
            {
                return Json(new { success = false, message = $"Bu etiketi kullanan {etiketBlogSayisi} blog var! Önce bloglardan kaldırmalısınız." });
            }

            _db.Etiketlers.Remove(etiket);
            _db.SaveChanges();

            return Json(new { success = true, message = "Etiket silindi!" });
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

        [HttpGet]
        public IActionResult Raporlar()
        {
            return View();
        }


    }
}