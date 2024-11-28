using MassTransit;
using Microsoft.Extensions.Hosting;
using Nerd.DocumentWorker.Consumers;


class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<ControlsConsumer>();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("rabbitmq://localhost:5672");
                        cfg.ReceiveEndpoint("eventQueue", e =>
                        {
                            e.Consumer<ControlsConsumer>(context);
                        });
                    });
                });
            });
}
