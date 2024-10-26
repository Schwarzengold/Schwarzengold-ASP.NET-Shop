using Microsoft.EntityFrameworkCore;
using Web_Menu.Models;
using WebMenu.DataAccess.Configurations;

namespace Web_Menu.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GameConfiguration());
        }
        public DbSet<Game> Games { get; set; }
        public DbSet<Character> Characters { get; set; }
    }
}
