using BlogWebsite.Models;

namespace BlogWebsite.Modeller
{
    public class AnasayfaBlogs
    {
        public List<BlogYazilari> BlogYazilarim { get; set; }
        public List<Etiketler> Etiketlerim { get; set; }
        public List<Kategoriler> Kategorilerim { get; set; }
        public List<Yorumlar> Yorumlarim { get; set; }
        public List<Kullanicilar> Kullanicilarim { get; set; }

    }
}
