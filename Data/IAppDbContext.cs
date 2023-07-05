using Microsoft.EntityFrameworkCore;
using OneTimePasswordManager.Models;

namespace OneTimePasswordManager.Data
{
    public interface IAppDbContext
    {
        DbSet<ValidPassword> Passwords { get; set; }
        int SaveChanges();
    }
}
