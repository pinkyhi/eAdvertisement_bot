using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eAdvertisement_bot.Models.DbEntities;
using Microsoft.EntityFrameworkCore;


namespace eAdvertisement_bot.DAO
{
    public class AppDbContext : DbContext
    {
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Advertisement_Status> Advertisement_Statuses { get; set; }
        public DbSet<Autobuy> Autobuys { get; set; }
        public DbSet<Autobuy_Channel> Autobuy_Channels { get; set; }
        public DbSet<Button> Buttons { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Channel_Category> Channel_Categories { get; set; }
        public DbSet<Input> Inputs { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=localhost;UserId=root;Password=df443335;database=eadvertisement_bot;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //FluentAPI

            modelBuilder.Entity<Advertisement>().HasKey(p => new { p.Date_Time, p.Channel_Id });

            // Many to Many

            modelBuilder.Entity<Autobuy_Channel>()
                .HasKey(cg => new { cg.Autobuy_Id, cg.Channel_Id });
            modelBuilder.Entity<Autobuy_Channel>()
                .HasOne(cg => cg.Autobuy)
                .WithMany(g => g.Autobuy_Channels)
                .HasForeignKey(cg => cg.Autobuy_Id);
            modelBuilder.Entity<Autobuy_Channel>()
                .HasOne(cg => cg.Channel)
                .WithMany(c => c.Autobuy_Channels)
                .HasForeignKey(cg => cg.Channel_Id);

            modelBuilder.Entity<Channel_Category>()
                .HasKey(og => new { og.Channel_Id, og.Category_Id });
            modelBuilder.Entity<Channel_Category>()
                .HasOne(og => og.Channel)
                .WithMany(g => g.Channel_Categories)
                .HasForeignKey(og => og.Channel_Id);
            modelBuilder.Entity<Channel_Category>()
                .HasOne(og => og.Category)
                .WithMany(o => o.Channel_Categories)
                .HasForeignKey(og => og.Category_Id);

            // Cascade deleting
            modelBuilder.Entity<Button>()
                .HasOne(b=>b.Publication)
                .WithMany(p=>p.Buttons)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Media>()
                .HasOne(m => m.Publication)
                .WithMany(p => p.Media)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Channel_Category>()     // Channel deleting
                .HasOne(cg => cg.Channel)
                .WithMany(c => c.Channel_Categories)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Autobuy_Channel>()
                .HasOne(a => a.Channel)
                .WithMany(c => c.Autobuy_Channels)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Place>()
                .HasOne(p => p.Channel)
                .WithMany(c => c.Places)
                .OnDelete(DeleteBehavior.Cascade);      // End of channel deleting
        }
    }
}
