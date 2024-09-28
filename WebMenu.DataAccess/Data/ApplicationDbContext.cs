using Microsoft.EntityFrameworkCore;
using Web_Menu.Models;

namespace Web_Menu.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Character> Characters { get; set; }
    }
}
