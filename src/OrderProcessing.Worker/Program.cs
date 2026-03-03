using OrderProcessing.Infrastructure;
using OrderProcessing.Application;

var builder = Host.CreateApplicationBuilder(args);

//builder.Configuration.AddJsonFile("appsettings.json", optional: false);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<OrdersProcessingWorker>();

var host = builder.Build();
host.Run();
