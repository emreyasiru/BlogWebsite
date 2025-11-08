namespace BlogWebsite.Models;

public partial class Etiketler
{
    public int EtiketId { get; set; }

    public string EtiketAdi { get; set; } = null!;

    public int KullanimSayisi { get; set; }
    public virtual ICollection<BlogEtiketleri> BlogEtiketleris { get; set; } = new List<BlogEtiketleri>();
}
