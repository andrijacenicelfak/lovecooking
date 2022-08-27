using Microsoft.EntityFrameworkCore;

namespace Models
{

    public class LoveCookingContext : DbContext
    {
        public DbSet<Korisnik>? Korisnici { get; set; }
        public DbSet<Jelo>? Jela { get; set; }
        public DbSet<Komentar>? Komentari { get; set; }
        public DbSet<Korak>? Koraci { get; set; }
        public DbSet<Meni>? Meniji { get; set; }
        public DbSet<Pice>? Pica { get; set; }
        public DbSet<Recept>? Recepti { get; set; }
        public DbSet<Sadrzi>? Sadrzi { get; set; }
        public DbSet<Sastojak>? Sastojci { get; set; }
        public DbSet<Slika>? Slike { get; set; }
        public DbSet<Takmicenje>? Takmicenja { get; set; }
        public DbSet<Ucestvovanje>? Ucestvovanja { get; set; }
        public DbSet<Prati>? Pracenja { get; set; }
        public LoveCookingContext(DbContextOptions context) : base(context) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Korisnik>()
            .HasMany(p => p.Prati).WithOne(p => p.Pratioc);
            builder.Entity<Korisnik>()
            .HasMany(p => p.Pratioci).WithOne(p => p.Pracenik);

            builder.Entity<Korisnik>().HasIndex(p => p.Email).IsUnique();
            builder.Entity<Korisnik>().HasIndex(p => p.Username).IsUnique();
        }
    }
}