using Microsoft.EntityFrameworkCore;
using UserService.Model;

namespace UserService.Data
{
    public class AppDbContext : DbContext
    {
        //public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        //{

        //}

        public DbSet<Users> Users { get; set; } // Updated to PascalCase for better convention


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=user-service;Database=mypostgresdb;Username=mysecretuser;Password=mysecretpassword;");
        }


        // No need for the OnConfiguring method, as the options will be injected through the constructor
    }
}
