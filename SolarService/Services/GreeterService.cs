using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using SolarService.Misc;
using SolarService.Models;  

namespace SolarService
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private bool powerInMW = true;
        private readonly SolarContext db = new SolarContext();
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override async Task GetUsersAsync(EmptyRequest request, IServerStreamWriter<User> responseStream, ServerCallContext context)
        {
            try
            {
                List<User> users = db.Users.ToList();

                foreach (User user in users)
                {
                    await responseStream.WriteAsync(user);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                await responseStream.WriteAsync(new User());
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

        public override Task<SuccessResponse> AuthoriseUser(AuthRequest request, ServerCallContext context)
        {
            try
            {
                string uName = Encryptor.Decrypt(request.UserName, "londoniron");
                string uPass = Encryptor.Decrypt(request.Password, "londoniron");
                bool userExists = db.Users.Where(x => x.Login == uName && x.Password == uPass).Any();
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Task.FromResult(new SuccessResponse()
                {
                    Success = false
                });
            }
        }

        public override async Task GetTelegramAuthorisedUsersAsync(EmptyRequest request, IServerStreamWriter<TelegramAuthorisedUser> responseStream, ServerCallContext context)
        {
            try
            {
                List<TelegramAuthorisedUser> users = db.TelegramAuthorisedUsers.ToList();

                foreach (TelegramAuthorisedUser user in users)
                {
                    await responseStream.WriteAsync(user);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                await responseStream.WriteAsync(new TelegramAuthorisedUser());
            }
        }

        public override async Task GetEventTypesAsync(EmptyRequest request, IServerStreamWriter<EventType> responseStream, ServerCallContext context)
        {
            try
            {
                List<EventType> eventTypes = db.EventTypes.ToList();

                foreach (EventType item in eventTypes)
                {
                    await responseStream.WriteAsync(item);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                await responseStream.WriteAsync(new EventType());
            }
        }

        public override async Task GetStationsAsync(EmptyRequest request, IServerStreamWriter<SolarStation> responseStream, ServerCallContext context)
        {
            try
            {
                List<SolarStation> stations = db.SolarStations.ToList();

                foreach (SolarStation item in stations)
                {
                    await responseStream.WriteAsync(item);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                await responseStream.WriteAsync(new SolarStation());
            }
        }

        public override Task<MeteoStation> GetStationForecast(StationRequest request, ServerCallContext context)
        {
            try
            {
                var forecast = db.MeteoStations.Where(x => x.StationId == request.StationId).FirstOrDefault();
                return Task.FromResult(forecast);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Task.FromResult(new MeteoStation());
        }

        public override async Task GetAllInvertorsAsync(EmptyRequest request, IServerStreamWriter<Invertor> responseStream, ServerCallContext context)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await responseStream.WriteAsync(new Invertor());
            }
        }

        public override async Task GetInvertorsOnStationAsync(InvertorsOnStationRequest request, IServerStreamWriter<Invertor> responseStream, ServerCallContext context)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await responseStream.WriteAsync(new Invertor());
            }
        }

        public override async Task GetErrorTypesAsync(EmptyRequest request, IServerStreamWriter<ErrorType> responseStream, ServerCallContext context)
        {
            try
            {
                List<ErrorType> errorTypes = db.ErrorTypes.ToList();

                foreach (ErrorType item in errorTypes)
                {
                    await responseStream.WriteAsync(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await responseStream.WriteAsync(new ErrorType());
            }
        }

        public override async Task GetStationStatisticForChartAsync(StationProducingStatisticRequest request, IServerStreamWriter<InvertorProducingStatistic> responseStream, ServerCallContext context)
        {try
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                await responseStream.WriteAsync(new InvertorProducingStatistic());
            }
        }

        public override async Task GetAllStationsStatisticForChartAsync(PeriodRequest request, IServerStreamWriter<InvertorProducingStatistic> responseStream, ServerCallContext context)
        {
            try
            {
                List<InvertorProducingStatistic> statistics = db.InvertorProducingStatistics.Where(x => x.Date >= request.FromDate && x.Date <= request.ToDate).ToList();

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await responseStream.WriteAsync(new InvertorProducingStatistic());
            }
        }

        public override Task<InvertorProducingStatistic> GetAllStationsStatistics(EmptyRequest request, ServerCallContext context)
        {
            try
            {
                InvertorProducingStatistic result = new InvertorProducingStatistic();
                for (int i = 1; i <= 6; i++)
                {
                    InvertorProducingStatistic stationResult = null;
                    if (db.InvertorProducingStatistics.Where(x => x.StationId == i).ToList().Any())
                        stationResult = db.InvertorProducingStatistics.Where(x => x.StationId == i).ToList().Last();
                    if (stationResult != null)
                    {
                        stationResult.ErrorCount = db.Events.Where(x => x.StationId == i).Count();
                        result.ProducedEnergy += stationResult.ProducedEnergy;
                        result.PredictedProducing += stationResult.PredictedProducing;
                        result.ActivePower += stationResult.ActivePower;
                        result.ErrorCount += stationResult.ErrorCount;
                    }
                }
                if (powerInMW)
                {
                    result.PredictedProducing /= 1000;
                    result.ActivePower /= 1000;
                    result.ProducedEnergy /= 1000;
                }
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Task.FromResult(new InvertorProducingStatistic());
        }

        public override Task<InvertorProducingStatistic> GetStationStatisticAsync(StationProducingStatisticRequest request, ServerCallContext context)
        {
            try
            {
                InvertorProducingStatistic statisticsNow = null;
                if (db.InvertorProducingStatistics.Where(x => x.StationId == request.StationId).ToList().Any())
                    statisticsNow = db.InvertorProducingStatistics.Where(x => x.StationId == request.StationId).ToList().Last();

                InvertorProducingStatistic statisticsOnRequestDate = null;
                if (db.InvertorProducingStatistics.Where(x => x.StationId == request.StationId && x.Date == request.Date).ToList().Any())
                    statisticsOnRequestDate = db.InvertorProducingStatistics.Where(x => x.StationId == request.StationId && x.Date == request.Date).ToList().Last();

                InvertorProducingStatistic statisticsResult = statisticsNow;
                if (statisticsResult != null)
                {
                    if (statisticsOnRequestDate != null)
                    {
                        statisticsResult.PredictedProducing -= statisticsOnRequestDate.PredictedProducing;
                        statisticsResult.ActivePower -= statisticsOnRequestDate.ActivePower;
                        statisticsResult.ProducedEnergy -= statisticsOnRequestDate.ProducedEnergy;
                    }
                    statisticsResult.ErrorCount = db.Events.Where(x => x.StationId == statisticsResult.StationId && x.Date >= request.Date).Count();
                    if (powerInMW)
                    {
                        statisticsResult.PredictedProducing /= 1000;
                        statisticsResult.ActivePower /= 1000;
                        statisticsResult.ProducedEnergy /= 1000;
                    }
                    return Task.FromResult(statisticsResult);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Task.FromResult(new InvertorProducingStatistic());
        }

        public override Task<InvertorProducingStatistic> GetInvertorStatisticsAsync(InvertorProducingStatisticRequest request, ServerCallContext context)
        {
            try
            {
                InvertorProducingStatistic statisticsNow = db.InvertorProducingStatistics.Where(x => x.InvertorId == request.InvertorId && x.Date >= request.FromDate).First();
                if (powerInMW)
                {
                    statisticsNow.PredictedProducing /= 1000;
                    statisticsNow.ActivePower /= 1000;
                    statisticsNow.ProducedEnergy /= 1000;
                }
                return Task.FromResult(statisticsNow);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Task.FromResult(new InvertorProducingStatistic());
            }
        }

        public override async Task GetEventsAsync(EventsRequest request, IServerStreamWriter<Event> responseStream, ServerCallContext context)
        {
            try
            {
                List<Event> events = db.Events.Where(x => x.Date >= request.FromDate && x.Date <= request.ToDate).ToList();

                events = FilterEvents(request, events);

                if (events.Count < 1) await responseStream.WriteAsync(new Event());

                foreach (Event item in events)
                {
                    await responseStream.WriteAsync(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await responseStream.WriteAsync(new Event());
            }
        }

        private List<Event> FilterEvents(EventsRequest request, List<Event> events)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.EventType))
                {
                    EventType type = db.EventTypes.Where(x => x.Name == request.EventType).FirstOrDefault();
                    events = events.Where(x => x.EventTypeId == type.Id).ToList();
                }

                if (!string.IsNullOrEmpty(request.StationName))
                {
                    SolarStation station = db.SolarStations.Where(x => x.Name == request.StationName).FirstOrDefault();
                    events = events.Where(x => x.StationId == station.Id).ToList();
                }
                else if (!string.IsNullOrEmpty(request.InvertorName))
                {
                    Invertor invertor = db.Invertors.Where(x => x.Name == request.InvertorName).FirstOrDefault();
                    events = events.Where(x => x.StationId == invertor.Id).ToList();
                }
                else if (!string.IsNullOrEmpty(request.ErrorMessage))
                {
                    ErrorType type = db.ErrorTypes.Where(x => x.Name == request.ErrorMessage).FirstOrDefault();
                    events = events.Where(x => x.ErrorTypeId == type.Id).ToList();
                }
                return events;
            }
            catch(NullReferenceException) { return new List<Event>(); }
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

        public override Task<ErrorMessageResponse> GetErrorMessage(ErrorTypeRequest request, ServerCallContext context)
        {
            try
            {
                ErrorType response = db.ErrorTypes.Where(x => x.Id == request.ErrorTypeId).FirstOrDefault();
                return Task.FromResult(new ErrorMessageResponse()
                {
                    Message = response.Name,
                });
            }
            catch(NullReferenceException)
            {
                return Task.FromResult(new ErrorMessageResponse());
            }
        }

        public override async Task GetInvertorStatisticForChartAsync(InvertorProducingStatisticRequest request, IServerStreamWriter<InvertorProducingStatistic> responseStream, ServerCallContext context)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await responseStream.WriteAsync(new InvertorProducingStatistic());
            }
        }

        public override Task<Invertor> GetInvertor(InvertorRequest request, ServerCallContext context)
        {
            try
            {
                Invertor invertor = db.Invertors.Where(x => x.Id == request.InvertorId).FirstOrDefault();
                return Task.FromResult(invertor);
            }
            catch (NullReferenceException)
            {
                return Task.FromResult(new Invertor());
            }
        }

        public override Task<SolarStation> GetStation(StationRequest request, ServerCallContext context)
        {
            try
            {
                SolarStation station = db.SolarStations.Where(x => x.Id == request.StationId).FirstOrDefault();
                return Task.FromResult(station);
            }
            catch (NullReferenceException)
            {
                return Task.FromResult(new SolarStation());
            }
        }

    }
}

