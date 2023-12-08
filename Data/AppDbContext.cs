using Microsoft.EntityFrameworkCore;
using UserService.Model;

namespace UserService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options ) : base(options)
        {

        }

        public DbSet<Users> users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=mssqlstud.fhict.local;Database=dbi469980_userdb;User Id=dbi469980_userdb;Password=Xtt4d-8HNK; TrustServerCertificate=True;");
        }

    }
}
