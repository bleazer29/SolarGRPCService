using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Timers;
using Grpc.Core;
using Microcharts;
using Microsoft.Extensions.Logging;
using SolarService.Models;

namespace SolarService
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly SolarContext db = new SolarContext();
        private Timer changeDataTimer = new Timer(); // for testing purposes
        private bool powerInMW = true;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
            if (!changeDataTimer.Enabled)
            {
                changeDataTimer.Interval = 60000;
                changeDataTimer.Elapsed += UpdateChangingData;
                changeDataTimer.Start();
            }
        }

        private void UpdateChangingData(object sender, ElapsedEventArgs e)
        {
            UpdateTotalStatistics();
        }

        private void UpdateTotalStatistics()
        {
            foreach(var item in db.StationProducingStatistics.ToList())
            {
                item.ActivePower += 1534;
                item.ProducedEnergy += 1422;
            }
        }

        public override async Task GetUsersAsync(EmptyRequest request, IServerStreamWriter<User> responseStream, ServerCallContext context)
        {
            List<User> users = db.Users.ToList();

            foreach (User user in users)
            {
                await responseStream.WriteAsync(user);
            }
        }

        public override Task<SuccessResponse> RemoveUser(TelegramAuthorisedUser request, ServerCallContext context)
        {
            try
            {
                db.TelegramAuthorisedUsers.Remove(request);
                User temp = db.Users.Where(x => x.Login == request.UserLogin).First();
                db.Users.Remove(temp);
                return Task.FromResult(new SuccessResponse()
                {
                    Success = true
                });
            }
            catch
            {

                return Task.FromResult(new SuccessResponse()
                {
                    Success = false
                });
            }
        }

        public override async Task GetRolesAsync(EmptyRequest request, IServerStreamWriter<Role> responseStream, ServerCallContext context)
        {
            List<Role> roles = db.Roles.ToList();

            foreach (Role role in roles)
            {
                await responseStream.WriteAsync(role);
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
                if (powerInMW)
                {
                    item.ProducedEnergy /= 1000;
                }
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetInvertorsOnStationAsync(InvertorsOnStationRequest request, IServerStreamWriter<Invertor> responseStream, ServerCallContext context)
        {
            List<Invertor> invertors = db.Invertors.Where(x => x.StationId == request.StationId).ToList();

            foreach (Invertor item in invertors)
            {
                if (powerInMW)
                {
                    item.ProducedEnergy /= 1000;
                }
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetErrorTypesAsync(EmptyRequest request, IServerStreamWriter<ErrorType> responseStream, ServerCallContext context)
        {
            List<ErrorType> errorTypes = db.ErrorTypes.ToList();

            foreach (ErrorType item in errorTypes)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetAllStationProducingStatisticsAsync(EmptyRequest request, IServerStreamWriter<StationProducingStatistic> responseStream, ServerCallContext context)
        {
            List<StationProducingStatistic> errorTypes = db.StationProducingStatistics.ToList();

            foreach (StationProducingStatistic item in errorTypes)
            {
                if (powerInMW)
                {
                    item.ProducedEnergy /= 1000;
                    item.PredictedProducing /= 1000;
                }
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetStationProducingStatisticAsync(StationProducingStatisticRequest request, IServerStreamWriter<StationProducingStatistic> responseStream, ServerCallContext context)
        {
            List<StationProducingStatistic> errorTypes = db.StationProducingStatistics.Where(x => x.StationId == request.StationId).ToList();

            foreach (StationProducingStatistic item in errorTypes)
            {
                if (powerInMW)
                {
                    item.ProducedEnergy /= 1000;
                    item.PredictedProducing /= 1000;
                }
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetEventsByErrorCode(ErrorCodeRequest request, IServerStreamWriter<Event> responseStream, ServerCallContext context)
        {
            List<Event> events = db.Events.Where(x => x.ErrorCode == request.Code).ToList();

            foreach (Event item in events)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetEventsByInvertor(InvertorRequest request, IServerStreamWriter<Event> responseStream, ServerCallContext context)
        {
            List<Event> events = db.Events.Where(x => x.InvertorId == request.InvertorId).ToList();

            foreach (Event item in events)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetEventsByStation(StationRequest request, IServerStreamWriter<Event> responseStream, ServerCallContext context)
        {
            List<Event> events = db.Events.Where(x => x.StationId == request.StationId).ToList();

            foreach (Event item in events)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override Task<SuccessResponse> ChangePowerMeasurementUnit(IsMWRequest request, ServerCallContext context)
        {
            try
            {
                powerInMW = request.IsMW;
                return Task.FromResult(new SuccessResponse()
                {
                    Success = true
                });
            }
            catch
            {
                return Task.FromResult(new SuccessResponse()
                {
                    Success = false
                });
            }
        }

        public override async Task GetStationTotalStatisticsAsync(EmptyRequest request, IServerStreamWriter<TotalStationProductionStatistics> responseStream, ServerCallContext context)
        {
            List<StationProducingStatistic> statistics = db.StationProducingStatistics.ToList();

            List<TotalStationProductionStatistics> totalStatistics = new List<TotalStationProductionStatistics>();
            TotalStationProductionStatistics station1Statistics = new TotalStationProductionStatistics()
            {
                StationId = 1,
                ErrorCount = db.Events.Where(x => x.StationId == 1).Count()
            };
            TotalStationProductionStatistics station2Statistics = new TotalStationProductionStatistics()
            {
                StationId = 2,
                ErrorCount = db.Events.Where(x => x.StationId == 2).Count()
            };
            TotalStationProductionStatistics station3Statistics = new TotalStationProductionStatistics()
            {
                StationId = 3,
                ErrorCount = db.Events.Where(x => x.StationId == 3).Count()
            };
            TotalStationProductionStatistics station4Statistics = new TotalStationProductionStatistics()
            {
                StationId = 4,
                ErrorCount = db.Events.Where(x => x.StationId == 4).Count()
            };
            TotalStationProductionStatistics station5Statistics = new TotalStationProductionStatistics()
            {
                StationId = 5,
                ErrorCount = db.Events.Where(x => x.StationId == 5).Count()
            };
            TotalStationProductionStatistics station6Statistics = new TotalStationProductionStatistics()
            {
                StationId = 6,
                ErrorCount = db.Events.Where(x => x.StationId == 6).Count()
            };
            foreach (var item in statistics)
            {
                switch (item.StationId)
                {
                    case 1:
                        station1Statistics.ProducedEnergy += item.ProducedEnergy;
                        station1Statistics.PredictedProducing += item.PredictedProducing;
                        station1Statistics.ActivePower += item.ActivePower;
                        break;
                    case 2:
                        station2Statistics.ProducedEnergy += item.ProducedEnergy;
                        station2Statistics.PredictedProducing += item.PredictedProducing;
                        station2Statistics.ActivePower += item.ActivePower;
                        break;
                    case 3:
                        station3Statistics.ProducedEnergy += item.ProducedEnergy;
                        station3Statistics.PredictedProducing += item.PredictedProducing;
                        station3Statistics.ActivePower += item.ActivePower;
                        break;
                    case 4:
                        station4Statistics.ProducedEnergy += item.ProducedEnergy;
                        station4Statistics.PredictedProducing += item.PredictedProducing;
                        station4Statistics.ActivePower += item.ActivePower;
                        break;
                    case 5:
                        station5Statistics.ProducedEnergy += item.ProducedEnergy;
                        station5Statistics.PredictedProducing += item.PredictedProducing;
                        station5Statistics.ActivePower += item.ActivePower;
                        break;
                    case 6:
                        station6Statistics.ProducedEnergy += item.ProducedEnergy;
                        station6Statistics.PredictedProducing += item.PredictedProducing;
                        station6Statistics.ActivePower += item.ActivePower;
                        break;
                    default:
                        _logger.LogError("Error on loading total station statistics");
                        break;
                }
            }
            totalStatistics.Add(station1Statistics);
            totalStatistics.Add(station2Statistics);
            totalStatistics.Add(station3Statistics);
            totalStatistics.Add(station4Statistics);
            totalStatistics.Add(station5Statistics);
            totalStatistics.Add(station6Statistics);
            foreach (var item in totalStatistics)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override Task<ChartImage> GetStatisticsChartImage(EmptyRequest request, ServerCallContext context)
        {
            var statistics = db.StationProducingStatistics.ToList();
            Entry[] chartEntries = new Entry[statistics.Count * 2];
            int i = 0;
            foreach(var item in statistics)
            {
                Entry tempProducing = new Entry((float)item.ProducedEnergy)
                {
                    Label = "Energy",
                    ValueLabel = item.ProducedEnergy.ToString()
                };
                Entry tempPrediction = new Entry((float)item.PredictedProducing)
                {
                    Label = "Prediction",
                    ValueLabel = item.PredictedProducing.ToString()
                };
                chartEntries[i++] = tempProducing;
                chartEntries[i++] = tempPrediction;
            }
            LineChart chart = new LineChart() { Entries = chartEntries }; 
            BinaryFormatter bf = new BinaryFormatter();
            byte[] chartImage;
            string res;
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, chart);
                chartImage = ms.ToArray();
                res = chartImage.ToString();
            };
            return Task.FromResult(new ChartImage()
            {
                 Image = res
            });
        }

    }
}

