using Microsoft.EntityFrameworkCore;
using OneTimePasswordManager.Models;

namespace OneTimePasswordManager.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ValidPassword> Passwords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ValidPassword>()
                        .HasKey(vp => new { vp.UserId, vp.Password });
        }
    }
}
