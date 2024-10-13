using Microsoft.EntityFrameworkCore;
using Web_Menu.Models;

namespace Web_Menu.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .Property(g => g.Price)
                .HasColumnType("decimal(18,2)"); 
        }
        public DbSet<Game> Games { get; set; }
        public DbSet<Character> Characters { get; set; }
    }
}
