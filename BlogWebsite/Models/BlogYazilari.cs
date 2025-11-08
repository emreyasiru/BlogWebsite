namespace BlogWebsite.Models;

public partial class BlogYazilari
{
    public int BlogId { get; set; }

    public string Baslik { get; set; } = null!;

    public string? Ozet { get; set; }

    public string Icerik { get; set; } = null!;

    public string? AnaGorsel { get; set; }

    public int KategoriId { get; set; }

    public int YazarId { get; set; }

    public int GoruntulemeSayisi { get; set; }

    public int Durum { get; set; }

    public DateTime? YayinTarihi { get; set; }

    public virtual ICollection<BlogEtiketler> BlogEtiketlers { get; set; } = new List<BlogEtiketler>();

    public virtual Kategoriler Kategori { get; set; } = null!;

    public virtual Kullanicilar Yazar { get; set; } = null!;

    public virtual ICollection<Yorumlar> Yorumlars { get; set; } = new List<Yorumlar>();

}
