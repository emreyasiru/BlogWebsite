using System;
using System.Collections.Generic;

namespace BlogWebsite.Models;

public partial class BlogEtiketler
{
    public int Id { get; set; }

    public int BlogId { get; set; }

    public int EtiketId { get; set; }

    public virtual BlogYazilari Blog { get; set; } = null!;

    public virtual Etiketler Etiket { get; set; } = null!;
}
