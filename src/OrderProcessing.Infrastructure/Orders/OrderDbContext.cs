using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Domain.Orders;
using OrderProcessing.Infrastructure.Orders;

namespace OrderProcessing.Infrastructure.Orders;

public class OrderDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }
}