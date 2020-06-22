using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using SolarService.Models;

namespace SolarService
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private SolarContext db;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
            db = new SolarContext();
            Console.WriteLine("Service started. Database created");
        }

        public override async Task GetUsersAsync(EmptyRequest request, IServerStreamWriter<User> responseStream, ServerCallContext context)
        {
            List<User> users = db.Users.ToList();

            foreach(User user in users)
            {
                await responseStream.WriteAsync(user);
            }
        }

        public override async Task GetTelegramAuthorisedUsersAsync(EmptyRequest request, IServerStreamWriter<TelegramAuthorisedUser> responseStream, ServerCallContext context)
        {
            List<TelegramAuthorisedUser> users = db.TelegramAuthorisedUsers.ToList();

            foreach (TelegramAuthorisedUser user in users)
            {
                await responseStream.WriteAsync(user);
            }
        }

        public override async Task GetEventsAsync(EmptyRequest request, IServerStreamWriter<Event> responseStream, ServerCallContext context)
        {
            List<Event> events = db.Events.ToList();

            foreach (Event item in events)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetEventTypesAsync(EmptyRequest request, IServerStreamWriter<EventType> responseStream, ServerCallContext context)
        {
            List<EventType> eventTypes = db.EventTypes.ToList();

            foreach (EventType item in eventTypes)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetEventsByTypeAsync(EventsByTypeRequest request, IServerStreamWriter<Event> responseStream, ServerCallContext context)
        {
            List<Event> events = db.Events.Where(x => x.EventTypeId == request.TypeId).ToList();

            foreach (Event item in events)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetStationsAsync(EmptyRequest request, IServerStreamWriter<SolarStation> responseStream, ServerCallContext context)
        {
            List<SolarStation> stations = db.SolarStations.ToList();

            foreach (SolarStation item in stations)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetAllInvertorsAsync(EmptyRequest request, IServerStreamWriter<Invertor> responseStream, ServerCallContext context)
        {
            List<Invertor> invertors = db.Invertors.ToList();

            foreach (Invertor item in invertors)
            {
                await responseStream.WriteAsync(item);  
            }
        }

        public override async Task GetInvertorsOnStationAsync(InvertorsOnStationRequest request, IServerStreamWriter<Invertor> responseStream, ServerCallContext context)
        {
            List<Invertor> invertors = db.Invertors.Where(x => x.StationId == request.StationId).ToList();

            foreach (Invertor item in invertors)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public override Task<StationProducedEnergy> GetStationProducedEnergyAsync(InvertorsOnStationRequest request, ServerCallContext context)
        {
            List<Invertor> invertors = db.Invertors.Where(x => x.StationId == request.StationId).ToList();
            double totalEnergyOnStation = 0;
            foreach (Invertor item in invertors)
            {
                totalEnergyOnStation += item.ProducedEnergy;
            }
            return Task.FromResult(new StationProducedEnergy()
            {
                Energy = totalEnergyOnStation
            });
        }

        public override Task<StationProducedEnergy> GetTotalProducedEnergyAsync(EmptyRequest request, ServerCallContext context)
        {
            List<Invertor> invertors = db.Invertors.ToList();
            double totalEnergy = 0;
            foreach (Invertor item in invertors)
            {
                totalEnergy += item.ProducedEnergy;
            }
            return Task.FromResult(new StationProducedEnergy()
            {
                Energy = totalEnergy
            });
        }
    }
}
