using Microsoft.Extensions.DependencyInjection;
using OrderProcessing.Application.Orders.Create;
using OrderProcessing.Application.Orders.List;
using OrderProcessing.Application.Orders.ChangeStatus;

namespace OrderProcessing.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<ICreateOrderHandler, CreateOrderHandler>();
        services.AddScoped<IListOrderHandler, ListOrderHandler>();
        services.AddScoped<IChangeOrderStatusHandler, ChangeOrderStatusHandler>();
        

        return services;
    }
}