using System;
using System.Collections.Generic;

namespace BlogWebsite.Models;

public partial class Kullanicilar
{
    public int KullaniciId { get; set; }

    public string KullaniciAdi { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Sifre { get; set; } = null!;

    public string AdSoyad { get; set; } = null!;

    public string? ProfilResmi { get; set; }

    public string? Hakkinda { get; set; }

    public string Rol { get; set; } = null!;

    public bool Aktif { get; set; }

    public DateTime KayitTarihi { get; set; }

    public DateTime? SonGirisTarihi { get; set; }
}
