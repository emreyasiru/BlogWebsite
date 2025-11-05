using System;
using System.Collections.Generic;

namespace BlogWebsite.Models;

public partial class Kategoriler
{
    public int KategoriId { get; set; }

    public string KategoriAdi { get; set; } = null!;

    public virtual ICollection<BlogYazilari> BlogYazilaris { get; set; } = new List<BlogYazilari>();
}
