using OrderProcessing.Infrastructure.Orders;
using OrderProcessing.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Application.Orders.Create;
using OrderProcessing.Application.Orders.List;
using OrderProcessing.Application.Orders.ChangeStatus;


var builder = WebApplication.CreateBuilder(args);
Console.WriteLine(builder.Configuration["ConnectionStrings:Default"]);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<OrderDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICreateOrderHandler, CreateOrderHandler>();
builder.Services.AddScoped<IListOrderHandler, ListOrderHandler>();
builder.Services.AddScoped<IChangeOrderStatusHandler, ChangeOrderStatusHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapGet("/health", () => Results.Ok("Healthy"));
app.Run();

