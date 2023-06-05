using FileCreateWorkerService.Services;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace FileCreateWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    IConfiguration configuration = context.Configuration;

                    services.AddSingleton<RabbitMQClientService>();
                    services.AddSingleton(sp => new ConnectionFactory()
                    {
                        Uri = new Uri(configuration.GetConnectionString("RabbitMQ")),
                        DispatchConsumersAsync = true
                    });

                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}