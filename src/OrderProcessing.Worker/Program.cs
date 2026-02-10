using OrderProcessing.Infrastructure.Orders;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

//builder.Configuration.AddJsonFile("appsettings.json", optional: false);

builder.Services.AddDbContext<OrderDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);

builder.Services.AddHostedService<OrdersProcessingWorker>();

var host = builder.Build();
host.Run();
