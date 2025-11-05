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

    public string Rol { get; set; } = null!;
}
