using Microsoft.EntityFrameworkCore;
using Hinario.Domain.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Hinario.Infra.Context
{
    public class HinarioApiContext : DbContext
    {
        public HinarioApiContext(DbContextOptions<HinarioApiContext> options)
            : base(options)
        {
        }

        public DbSet<Hino> Hinos { get; set; } = null!;
        public DbSet<Repertorio> Repertorios { get; set; } = null!;
        public DbSet<RepertorioItem> RepertorioItens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hino>()
                .Property(h => h.Id)
                .ValueGeneratedOnAdd()
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity<Hino>()
                .HasIndex(h => h.Identificador)
                .IsUnique()
                .HasFilter("\"identificador\" IS NOT NULL");

            modelBuilder.Entity<Hino>(entity =>
            {
                entity.Property(h => h.LetraIdx)
                    .HasColumnName("letra_idx")
                    .HasColumnType("tsvector")
                    .ValueGeneratedOnAddOrUpdate(); // coluna gerada pelo banco

                entity.HasIndex(h => h.LetraIdx)
                    .HasMethod("GIN");
            });

            modelBuilder.Entity<RepertorioItem>(entity =>
            {
                entity.HasOne(i => i.Repertorio)
                    .WithMany(r => r.Itens)
                    .HasForeignKey(i => i.RepertorioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(i => i.Hino)
                    .WithMany()
                    .HasForeignKey(i => i.HinoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(i => new { i.RepertorioId, i.Ordem })
                    .IsUnique();
            });
        }
    }
}
