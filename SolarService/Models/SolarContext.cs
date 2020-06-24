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
            Database.EnsureCreated();
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
                new Invertor { Id=1, Name="Invertor1", Current=12.3, ActivePower=100.5, ProducedEnergy=130.85, State="Working", StationId = st1.Id, Temperature=26.4, ACStringVoltage=20.9, DCStringVoltage=19.5, AC=16.3, DC=18.4, Frequency=12.0},
                new Invertor { Id=2, Name="Invertor2", Current=16.3, ActivePower=180.5, ProducedEnergy=160.85, State="Working", StationId = st1.Id, Temperature=29.4, ACStringVoltage=21.9, DCStringVoltage=18.5, AC=16.3, DC=18.4, Frequency=12.0},

                new Invertor { Id=3, Name="Invertor3", Current=17.3, ActivePower=230.5, ProducedEnergy=110.85, State="Working", StationId = st2.Id, Temperature=29.0, ACStringVoltage=22.9, DCStringVoltage=13.5, AC=16.3, DC=18.4, Frequency=12.0},
                new Invertor { Id=4, Name="Invertor4", Current=14.3, ActivePower=170.5, ProducedEnergy=167.85, State="Working", StationId = st2.Id, Temperature=26.4, ACStringVoltage=20.9, DCStringVoltage=15.5, AC=16.3, DC=18.4, Frequency=12.0},

                new Invertor { Id=5, Name="Invertor5", Current=11.3, ActivePower=210.5, ProducedEnergy=110.85, State="Working", StationId = st3.Id, Temperature=29.4, ACStringVoltage=21.9, DCStringVoltage=11.5, AC=16.3, DC=18.4, Frequency=12.0},
                new Invertor { Id=6, Name="Invertor6", Current=19.3, ActivePower=230.5, ProducedEnergy=251.85, State="Working", StationId = st3.Id, Temperature=29.0, ACStringVoltage=22.9, DCStringVoltage=16.5, AC=16.3, DC=18.4, Frequency=12.0},

                new Invertor { Id=7, Name="Invertor7", Current=21.3, ActivePower=131.5, ProducedEnergy=190.85, State="Working", StationId = st4.Id, Temperature=29.4, ACStringVoltage=21.9, DCStringVoltage=11.4, AC=16.3, DC=18.4, Frequency=12.0},
                new Invertor { Id=8, Name="Invertor8", Current=10.3, ActivePower=262.5, ProducedEnergy=251.85, State="Working", StationId = st4.Id, Temperature=29.0, ACStringVoltage=22.9, DCStringVoltage=17.5, AC=16.3, DC=18.4, Frequency=12.0},

                new Invertor { Id=9, Name="Invertor9", Current=24.3, ActivePower=350.5, ProducedEnergy=452.85, State="Working", StationId = st5.Id, Temperature=26.4, ACStringVoltage=20.9, DCStringVoltage=12.5, AC=16.3, DC=18.4, Frequency=12.0},
                new Invertor { Id=10, Name="Invertor10", Current=12.3, ActivePower=152.5, ProducedEnergy=453.85, State="Working", StationId = st5.Id, Temperature=29.4, ACStringVoltage=21.9, DCStringVoltage=14.5, AC=16.3, DC=18.4, Frequency=12.0},

                new Invertor { Id=11, Name="Invertor11", Current=15.3, ActivePower=189.5, ProducedEnergy=165.85, State="Working", StationId = st6.Id, Temperature=29.4, ACStringVoltage=21.9, DCStringVoltage=13.5, AC=16.3, DC=18.4, Frequency=12.0},
                new Invertor { Id=12, Name="Invertor12", Current=22.3, ActivePower=250.5, ProducedEnergy=143.85, State="Working", StationId = st6.Id, Temperature=29.0, ACStringVoltage=22.9, DCStringVoltage=19.9, AC=16.3, DC=18.4, Frequency=12.0}
                });

            Role r1 = new Role() { Id = 1, Name = "Admin", Station1Pass = true, Station2Pass = true, Station3Pass = true, Station4Pass = true, Station5Pass = true, Station6Pass = true };
            Role r2 = new Role() { Id = 2, Name = "User", Station1Pass = false, Station2Pass = true, Station3Pass = true, Station4Pass = true, Station5Pass = true, Station6Pass = true };

            modelBuilder.Entity<Role>().HasData(
                 new Role[] { r1, r2 });

            modelBuilder.Entity<User>().HasData(
                 new User[]
                 {
                new User { Id=1, Login="user1", Password="pass1", RoleId = r1.Id},
                new User { Id=2, Login="user2", Password="pass2", RoleId = r2.Id},
                new User { Id=3, Login="user3", Password="pass3", RoleId = r2.Id}
                 });

            modelBuilder.Entity<StationProducingStatistic>().HasData(
                new StationProducingStatistic { Id = 1, ProducedEnergy = 1, PredictedProducing = 5, Date = TimestampDateTimeConverter.DateTimeToUnixTimeStamp(DateTime.Now.AddDays(-3).ToUniversalTime()), StationId = 1 },
                    new StationProducingStatistic { Id = 2, PredictedProducing = 2, ProducedEnergy = 2, Date = TimestampDateTimeConverter.DateTimeToUnixTimeStamp(DateTime.Now.AddDays(-2).ToUniversalTime()), StationId = 2 },
                    new StationProducingStatistic { Id = 3, PredictedProducing = 3, ProducedEnergy = 3, Date = TimestampDateTimeConverter.DateTimeToUnixTimeStamp(DateTime.Now.AddDays(-1).ToUniversalTime()), StationId = 2 },
                    new StationProducingStatistic { Id = 4, PredictedProducing = 4, ProducedEnergy = 4, Date = TimestampDateTimeConverter.DateTimeToUnixTimeStamp(DateTime.Now.ToUniversalTime()), StationId = 2 }
                );

            modelBuilder.Entity<MeteoStation>().HasData(
                 new MeteoStation[]
                 {
                    new MeteoStation { Id=1, Temperature=25.6, WindSpeed=11.4, StationId = 1},
                     new MeteoStation { Id=2, Temperature=34.3, WindSpeed=16.7, StationId = 2},
                     new MeteoStation { Id=3, Temperature=13.4, WindSpeed=14.9, StationId = 3},
                     new MeteoStation { Id=4, Temperature=24.9, WindSpeed=4.1, StationId = 4},
                     new MeteoStation { Id=5, Temperature=29.7, WindSpeed=1.5, StationId = 5},
                     new MeteoStation { Id=6, Temperature=20.5, WindSpeed=23.0, StationId = 6}
                 });

        }

        public DbSet<SolarStation> SolarStations { get; set; }
        public DbSet<MeteoStation> MeteoStations { get; set; }
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
