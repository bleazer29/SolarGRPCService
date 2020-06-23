using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using SolarService.Misc;
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
            Database.EnsureCreatedAsync();
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
            SolarStation st4 = new SolarStation { Id = 4, Name = "Station4" };
            SolarStation st5 = new SolarStation { Id = 5, Name = "Station5" };
            SolarStation st6 = new SolarStation { Id = 6, Name = "Station6" };

            modelBuilder.Entity<SolarStation>().HasData(
                new SolarStation[] { st1, st2, st3, st4, st5, st6 });

            modelBuilder.Entity<EventType>().HasData(
                new EventType[]
                {
                new EventType { Id=1, Name="Error"},
                new EventType { Id=2, Name="Normal event"}
                });

            modelBuilder.Entity<ErrorType>().HasData(
                new ErrorType[]
                {
                new ErrorType { Id=1, Name="Не поступает ток на инвертор"},
                new ErrorType { Id=2, Name="Слишком высокая температура инвертора" },
                new ErrorType { Id=3, Name="Нет ответа от метеостанции" }
                });

            modelBuilder.Entity<Event>().HasData(
                new Event[]
                {
                    new Event {Id=1, Date = TimestampDateTimeConverter.DateTimeToUnixTimeStamp(DateTime.Now.ToUniversalTime()), ErrorCode = 123, ErrorTypeId=2, EventTypeId=1, StationId=1, InvertorId=2 },
                    new Event {Id=2, Date = TimestampDateTimeConverter.DateTimeToUnixTimeStamp(DateTime.Now.AddDays(-2).ToUniversalTime()), ErrorCode = 121, ErrorTypeId=1, EventTypeId=1, StationId=2, InvertorId=1 },
                    new Event {Id=3, Date = TimestampDateTimeConverter.DateTimeToUnixTimeStamp(DateTime.Now.ToUniversalTime()), ErrorCode = 124, ErrorTypeId=3, EventTypeId=1, StationId=2 }
                });
            modelBuilder.Entity<Invertor>().HasData(
                new Invertor[]
                {
                new Invertor { Id=1, Name="Invertor1", Current=12.3, Power=100.5, ProducedEnergy=130.85, State="Working", StationId = st1.Id, Temperature=26.4, Voltage=20.9},
                new Invertor { Id=2, Name="Invertor2", Current=16.3, Power=180.5, ProducedEnergy=160.85, State="Working", StationId = st1.Id, Temperature=29.4, Voltage=21.9},
                
                new Invertor { Id=3, Name="Invertor3", Current=17.3, Power=230.5, ProducedEnergy=110.85, State="Working", StationId = st2.Id, Temperature=29.0, Voltage=22.9},
                new Invertor { Id=4, Name="Invertor4", Current=14.3, Power=170.5, ProducedEnergy=167.85, State="Working", StationId = st2.Id, Temperature=26.4, Voltage=20.9},
                
                new Invertor { Id=5, Name="Invertor5", Current=11.3, Power=210.5, ProducedEnergy=110.85, State="Working", StationId = st3.Id, Temperature=29.4, Voltage=21.9},
                new Invertor { Id=6, Name="Invertor6", Current=19.3, Power=230.5, ProducedEnergy=251.85, State="Working", StationId = st3.Id, Temperature=29.0, Voltage=22.9},
               
                new Invertor { Id=7, Name="Invertor7", Current=21.3, Power=131.5, ProducedEnergy=190.85, State="Working", StationId = st4.Id, Temperature=29.4, Voltage=21.9},
                new Invertor { Id=8, Name="Invertor8", Current=10.3, Power=262.5, ProducedEnergy=251.85, State="Working", StationId = st4.Id, Temperature=29.0, Voltage=22.9},
                
                new Invertor { Id=9, Name="Invertor9", Current=24.3, Power=350.5, ProducedEnergy=452.85, State="Working", StationId = st5.Id, Temperature=26.4, Voltage=20.9},
                new Invertor { Id=10, Name="Invertor10", Current=12.3, Power=152.5, ProducedEnergy=453.85, State="Working", StationId = st5.Id, Temperature=29.4, Voltage=21.9},
                
                new Invertor { Id=11, Name="Invertor11", Current=15.3, Power=189.5, ProducedEnergy=165.85, State="Working", StationId = st6.Id, Temperature=29.4, Voltage=21.9},
                new Invertor { Id=12, Name="Invertor12", Current=22.3, Power=250.5, ProducedEnergy=143.85, State="Working", StationId = st6.Id, Temperature=29.0, Voltage=22.9}
                });

            Role r1 = new Role() { Id = 1, Name = "Admin", RootRights = false, Station1Pass = true, Station2Pass = true, Station3Pass = true, Station4Pass = true, Station5Pass = true, Station6Pass = true };
            Role r2 = new Role() { Id = 2, Name = "User", RootRights = false, Station1Pass = false, Station2Pass = true, Station3Pass = true, Station4Pass = true, Station5Pass = true, Station6Pass = true };
                
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
        public DbSet<ErrorType> ErrorTypes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<StationProducingStatistic> StationProducingStatistics { get; set; }
        public DbSet<TelegramAuthorisedUser> TelegramAuthorisedUsers { get; set; }
    }
}
