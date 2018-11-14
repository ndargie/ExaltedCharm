using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ExaltedCharm.Api.Entities
{
    public class CharmContext : DbContext
    {
        public CharmContext(DbContextOptions<CharmContext> options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KeywordCharm>().HasKey(t => new {t.CharmId, t.KeywordId});
            modelBuilder.Entity<KeywordCharm>()
                .HasOne(pt => pt.Charm)
                .WithMany(p => p.Keywords)
                .HasForeignKey(pt => pt.CharmId);
            modelBuilder.Entity<KeywordCharm>()
                .HasOne(pt => pt.Keyword)
                .WithMany(t => t.Charms)
                .HasForeignKey(pt => pt.KeywordId);
            modelBuilder.Entity<Charm>(entity => { entity.HasIndex(e => e.Name).IsUnique(); });
            modelBuilder.Entity<Duration>(entity => entity.HasIndex(d => d.Name).IsUnique());
            modelBuilder.Entity<CharmType>(entity => entity.HasIndex(c => c.Name).IsUnique());
            modelBuilder.Entity<ExaltedType>(entity => entity.HasIndex(e => e.Name).IsUnique());
        }

        public override int SaveChanges()
        {
            var entities = from e in ChangeTracker.Entries()
                where e.State == EntityState.Added
                      || e.State == EntityState.Modified
                select e.Entity;
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(entity, validationContext);
            }

            return base.SaveChanges();
        }

        public DbSet<CharmType> CharmTypes { get; set; }
        public DbSet<Charm> Charms { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<ExaltedType> ExaltedTypes { get; set; }
        public DbSet<KeywordCharm> KeywordCharms { get; set; }
        public DbSet<Duration> Durations { get; set; }
    }
}