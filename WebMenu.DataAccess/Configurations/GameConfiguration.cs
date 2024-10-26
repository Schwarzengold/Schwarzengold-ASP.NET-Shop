using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_Menu.Models;

namespace WebMenu.DataAccess.Configurations
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.HasKey(g => g.Id);

            builder.Property(g => g.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(g => g.Price)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(g => g.Characters)
                .WithOne(c => c.Game)
                .HasForeignKey(c => c.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
