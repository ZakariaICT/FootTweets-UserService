using Microsoft.EntityFrameworkCore;
using UserService.Model;

namespace UserService.Data
{
    public class AppDbContext : DbContext
    {
        //public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        //{

        //}

        public DbSet<Users> users { get; set; } // Updated to PascalCase for better convention


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=postgres;Database=mydatabase;Username=myuser;Password=mypassword;");
        }


        // No need for the OnConfiguring method, as the options will be injected through the constructor
    }
}
