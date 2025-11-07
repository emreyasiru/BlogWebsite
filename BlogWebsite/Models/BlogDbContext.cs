using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BlogWebsite.Models;

public partial class BlogDbContext : DbContext
{
    public BlogDbContext()
    {
    }

    public BlogDbContext(DbContextOptions<BlogDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BlogYazilari> BlogYazilaris { get; set; }

    public virtual DbSet<Etiketler> Etiketlers { get; set; }

    public virtual DbSet<Kategoriler> Kategorilers { get; set; }

    public virtual DbSet<Kullanicilar> Kullanicilars { get; set; }

    public virtual DbSet<Yorumlar> Yorumlars { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-9TAOJGA\\SQLEXPRESS;Database=BlogDb;User Id=sa;Password=123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlogYazilari>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__BlogYazi__54379E30E5F60BDD");

            entity.ToTable("BlogYazilari");

            entity.Property(e => e.AnaGorsel).HasMaxLength(255);
            entity.Property(e => e.Baslik).HasMaxLength(200);
            entity.Property(e => e.Ozet).HasMaxLength(500);
            entity.Property(e => e.YayinTarihi).HasColumnType("datetime");

            entity.HasOne(d => d.Kategori).WithMany(p => p.BlogYazilaris)
                .HasForeignKey(d => d.KategoriId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BlogYazilari_Kategori");

            entity.HasOne(d => d.Yazar).WithMany(p => p.BlogYazilaris)
                .HasForeignKey(d => d.YazarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BlogYazilari_Yazar");
        });

        modelBuilder.Entity<Etiketler>(entity =>
        {
            entity.HasKey(e => e.EtiketId).HasName("PK__Etiketle__3214EC076F700978");

            entity.ToTable("Etiketler");

            entity.HasIndex(e => e.EtiketAdi, "UQ__Etiketle__3A42154ECC20896E").IsUnique();

            entity.Property(e => e.EtiketAdi).HasMaxLength(50);
        });

        modelBuilder.Entity<Kategoriler>(entity =>
        {
            entity.HasKey(e => e.KategoriId).HasName("PK__Kategori__3214EC07A2CD7FEA");

            entity.ToTable("Kategoriler");

            entity.HasIndex(e => e.KategoriAdi, "UQ__Kategori__110FF79E1A2F41AE").IsUnique();

            entity.Property(e => e.KategoriAdi).HasMaxLength(100);
        });

        modelBuilder.Entity<Kullanicilar>(entity =>
        {
            entity.HasKey(e => e.KullaniciId).HasName("PK__Kullanic__E011F77B59C8A4F8");

            entity.ToTable("Kullanicilar");

            entity.HasIndex(e => e.KullaniciAdi, "UQ__Kullanic__5BAE6A75EF7E5994").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Kullanic__A9D105347D069A94").IsUnique();

            entity.Property(e => e.AdSoyad).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.KullaniciAdi).HasMaxLength(50);
            entity.Property(e => e.ProfilResmi).HasMaxLength(255);
            entity.Property(e => e.Rol)
                .HasMaxLength(20)
                .HasDefaultValue("Yazar");
            entity.Property(e => e.Sifre).HasMaxLength(255);
        });

        modelBuilder.Entity<Yorumlar>(entity =>
        {
            entity.HasKey(e => e.YorumId).HasName("PK__Yorumlar__F2BE14E86331DC9E");

            entity.ToTable("Yorumlar");

            entity.Property(e => e.Ad).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IpAdresi).HasMaxLength(50);
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.YorumIcerik).HasMaxLength(1000);

            entity.HasOne(d => d.Blog).WithMany(p => p.Yorumlars)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK_Yorumlar_Blog");

            entity.HasOne(d => d.UstYorum).WithMany(p => p.InverseUstYorum)
                .HasForeignKey(d => d.UstYorumId)
                .HasConstraintName("FK_Yorumlar_Ust");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
