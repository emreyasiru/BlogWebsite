using System;
using System.Collections.Generic;

namespace BlogWebsite.Models;

public partial class Yorumlar
{
    public int YorumId { get; set; }

    public int BlogId { get; set; }

    public string Ad { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string YorumIcerik { get; set; } = null!;

    public bool Onaylandi { get; set; }

    public string? IpAdresi { get; set; }

    public int? UstYorumId { get; set; }

    public DateTime OlusturmaTarihi { get; set; }

    public virtual BlogYazilari Blog { get; set; } = null!;

    public virtual ICollection<Yorumlar> InverseUstYorum { get; set; } = new List<Yorumlar>();

    public virtual Yorumlar? UstYorum { get; set; }
}
