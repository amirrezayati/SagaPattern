using Microsoft.EntityFrameworkCore;
using SagaPattern.Choreography.OrderApi.Models;

namespace SagaPattern.Choreography.OrderApi.EF
{
    public class OrderingContext : DbContext
    {
        public OrderingContext(DbContextOptions<OrderingContext> options) : base(options)
        {

        }

        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
