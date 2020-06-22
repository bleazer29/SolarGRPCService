using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarService.Models
{
    public class SolarContext : DbContext
    {
        public SolarContext()
        {
            if (Database.EnsureCreated())
            {

            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SolarDb;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SolarStation st1 = new SolarStation { Id = 1, Name = "Station1" };
            SolarStation st2 = new SolarStation { Id = 2, Name = "Station2" };
            SolarStation st3 = new SolarStation { Id = 3, Name = "Station3" };

            modelBuilder.Entity<SolarStation>().HasData(
                new SolarStation[] { st1, st2, st3 });

            modelBuilder.Entity<EventType>().HasData(
                new EventType[]
                {
                new EventType { Id=1, Name="Error"},
                new EventType { Id=2, Name="Normal event"}
                });
            //modelBuilder.Entity<Event>().HasData(
            //    new Event[]
            //    {
            //        new Event {Id=1, Description="Invertor temperature above the normal", Date= DateTime.Now.AddDays(-2), }
            //    });
            modelBuilder.Entity<Invertor>().HasData(
                new Invertor[]
                {
                new Invertor { Id=1, Name="Invertor1", Current=12.3, Power=100.5, ProducedEnergy=130.85, State="Working", StationId = st1.Id, Temperature=26.4, Voltage=20.9},
                new Invertor { Id=2, Name="Invertor2", Current=16.3, Power=180.5, ProducedEnergy=160.85, State="Working", StationId = st2.Id, Temperature=29.4, Voltage=21.9},
                new Invertor { Id=3, Name="Invertor3", Current=17.3, Power=200.5, ProducedEnergy=110.85, State="Working", StationId = st3.Id, Temperature=29.0, Voltage=22.9}
                });

            Role r1 = new Role() { Id = 1, Name = "Admin", RootRights = true };
            Role r2 = new Role() { Id = 2, Name = "User", RootRights = false };

            modelBuilder.Entity<Role>().HasData(
                 new Role[] { r1, r2 });

            modelBuilder.Entity<User>().HasData(
                 new User[]
                 {
                new User { Id=1, Login="user1", Password="pass1", RoleId = r1.Id},
                new User { Id=2, Login="user2", Password="pass2", RoleId = r2.Id},
                new User { Id=3, Login="user3", Password="pass3", RoleId = r2.Id}
                 });
        }

        public DbSet<SolarStation> SolarStations { get; set; }
        public DbSet<Invertor> Invertors { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<TelegramAuthorisedUser> TelegramAuthorisedUsers { get; set; }
    }
}
