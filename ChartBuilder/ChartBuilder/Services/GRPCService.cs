using Grpc.Core;
using SolarService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChartBuilder.Services
{
    public class GRPCService
    {
        static Channel channel;
        static Greeter.GreeterClient client;
        private GRPCService()
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            channel = new Channel("https://localhost:5001", ChannelCredentials.Insecure);
            client = new Greeter.GreeterClient(channel);
            Console.WriteLine("Service connected");
        }


    }
}
