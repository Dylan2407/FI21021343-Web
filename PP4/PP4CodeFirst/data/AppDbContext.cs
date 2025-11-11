using Microsoft.EntityFrameworkCore;
using PP4CodeFirst.Models;
using System.IO;

namespace PP4CodeFirst.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Title> Titles { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;

        // <-- Asegúrate de que este DbSet exista, el nombre debe coincidir con lo que usas en Program.cs
        public DbSet<TitleTag> TitleTags { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Ruta relativa a la carpeta del proyecto: ./data/books.db
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "data", "books.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapear la entidad TitleTag a la tabla "TitlesTags" en la BD (solicitado)
            modelBuilder.Entity<TitleTag>().ToTable("TitlesTags");

            // Forzar orden de columnas en Title: TitleId, AuthorId, TitleName
            modelBuilder.Entity<Title>(entity =>
            {
                entity.Property(e => e.TitleId).HasColumnOrder(0);
                entity.Property(e => e.AuthorId).HasColumnOrder(1);
                entity.Property(e => e.TitleName).HasColumnOrder(2);
            });

            // Opcional: marcar la relación many-to-many (si la quieres explícita)
            modelBuilder.Entity<TitleTag>()
                .HasOne(tt => tt.Title)
                .WithMany(t => t.TitleTags)
                .HasForeignKey(tt => tt.TitleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TitleTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.TitleTags)
                .HasForeignKey(tt => tt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
