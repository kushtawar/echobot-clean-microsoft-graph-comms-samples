using EchoBot;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using System.IO;

CreateDeploymentMarker();

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Echo Bot Service";
    })
    .ConfigureServices(services =>
    {
        LoggerProviderOptions.RegisterProviderOptions<
            EventLogSettings, EventLogLoggerProvider>(services);

        services.AddSingleton<IBotHost, BotHost>();

        services.AddHostedService<EchoBotWorker>();
    })
    .Build();

await host.RunAsync();

static void CreateDeploymentMarker()
{
    try
    {
        var folder = @"C:\speechlogs";
        Directory.CreateDirectory(folder);

        var filename = $"build-marker-{DateTime.UtcNow:yyyyMMddHHmmss}.txt";
        var path = Path.Combine(folder, filename);

        using var file = File.Create(path);
    }
    catch
    {
        // Ignore marker failures so service startup is unaffected.
    }
}
