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
            foreach(var item in db.InvertorProducingStatistics.ToList())
            {
                if (powerInMW)
                {
                    item.ActivePower += 15343;
                    item.ProducedEnergy += 14422;
                }
                else
                {
                    item.ActivePower += 1533;
                    item.ProducedEnergy += 1442;
                }
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

        public override Task<SuccessResponse> AuthoriseUser(User request, ServerCallContext context)
        {
            bool userExists = db.Users.Where(x => x.Login == request.Login && x.Password == request.Password).Any();
            if (userExists)
            {
                return Task.FromResult(new SuccessResponse()
                {
                    Success = true
                });
            }
            return Task.FromResult(new SuccessResponse()
            {
                Success = false
            });

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

        public override async Task GetAllStationProducingStatisticsAsync(EmptyRequest request, IServerStreamWriter<InvertorProducingStatistic> responseStream, ServerCallContext context)
        {
            List<InvertorProducingStatistic> statistics = db.InvertorProducingStatistics.ToList();

            foreach (InvertorProducingStatistic item in statistics)
            {
                if (powerInMW)
                {
                    item.ProducedEnergy /= 1000;
                    item.PredictedProducing /= 1000;
                }
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetStationProducingStatisticPeriod(StationProducingStatisticRequest request, IServerStreamWriter<InvertorProducingStatistic> responseStream, ServerCallContext context)
        {
            List<InvertorProducingStatistic> statistics = db.InvertorProducingStatistics.Where(x => x.StationId == request.StationId && x.Date >= request.Date).ToList();

            foreach (InvertorProducingStatistic item in statistics)
            {
                if (powerInMW)
                {
                    item.ProducedEnergy /= 1000;
                    item.PredictedProducing /= 1000;
                }
                await responseStream.WriteAsync(item);
            }
        }

        public override Task<InvertorProducingStatistic> GetStationProducingStatisticAsync(StationProducingStatisticRequest request, ServerCallContext context)
        {
            InvertorProducingStatistic statisticsNow = db.InvertorProducingStatistics.Where(x => x.StationId == request.StationId).First();
            InvertorProducingStatistic statisticsOnRequestDate = db.InvertorProducingStatistics.Where(x => x.StationId == request.StationId && x.Date == request.Date).First();
            InvertorProducingStatistic statisticsResult = statisticsNow;

            statisticsResult.PredictedProducing -= statisticsOnRequestDate.PredictedProducing;
            statisticsResult.ActivePower -= statisticsOnRequestDate.ActivePower;
            statisticsResult.ProducedEnergy -= statisticsOnRequestDate.ProducedEnergy;
            statisticsResult.ErrorCount = db.Events.Where(x => x.StationId == statisticsResult.StationId && x.Date >= request.Date).Count();
            if (powerInMW)
            {
                statisticsResult.PredictedProducing /= 1000;
                statisticsResult.ActivePower /= 1000;
                statisticsResult.ProducedEnergy /= 1000;
            }
            return Task.FromResult(statisticsResult);
        }

        public override Task<InvertorProducingStatistic> GetInvertorProducingStatisticsAsync(InvertorProducingStatisticRequest request, ServerCallContext context)
        {
            InvertorProducingStatistic statisticsNow = db.InvertorProducingStatistics.Where(x => x.Id == request.InvertorId).First();
            InvertorProducingStatistic statisticsOnRequestDate = db.InvertorProducingStatistics.Where(x => x.StationId == request.InvertorId && x.Date == request.FromDate).First();
            InvertorProducingStatistic statisticsResult = statisticsNow;

            statisticsResult.PredictedProducing -= statisticsOnRequestDate.PredictedProducing;
            statisticsResult.ActivePower -= statisticsOnRequestDate.ActivePower;
            statisticsResult.ProducedEnergy -= statisticsOnRequestDate.ProducedEnergy;
            statisticsResult.ErrorCount = db.Events.Where(x => x.StationId == statisticsResult.StationId && x.Date >= request.FromDate).Count();
            if (powerInMW)
            {
                statisticsResult.PredictedProducing /= 1000;
                statisticsResult.ActivePower /= 1000;
                statisticsResult.ProducedEnergy /= 1000;
            }
            return Task.FromResult(statisticsResult);
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

        public override async Task GetEventsByStation(StationProducingStatisticRequest request, IServerStreamWriter<Event> responseStream, ServerCallContext context)
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

        public override async Task GetInvertorProducingStatisticPeriod(InvertorProducingStatisticRequest request, IServerStreamWriter<InvertorProducingStatistic> responseStream, ServerCallContext context)
        {
            List<InvertorProducingStatistic> statistics = db.InvertorProducingStatistics.Where(x => x.InvertorId == request.InvertorId && x.Date >= request.FromDate).ToList();

            foreach (InvertorProducingStatistic item in statistics)
            {
                if (powerInMW)
                {
                    item.ProducedEnergy /= 1000;
                    item.PredictedProducing /= 1000;
                }
                await responseStream.WriteAsync(item);
            }
        }

        public override Task<ChartImage> GetStatisticsChartImage(EmptyRequest request, ServerCallContext context)
        {
            var statistics = db.InvertorProducingStatistics.ToList();
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

