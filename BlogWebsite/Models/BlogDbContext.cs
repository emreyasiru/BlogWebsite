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

    public virtual DbSet<Kullanicilar> Kullanicilars { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-9TAOJGA\\SQLEXPRESS;Database=BlogDb;User Id=sa;Password=123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
