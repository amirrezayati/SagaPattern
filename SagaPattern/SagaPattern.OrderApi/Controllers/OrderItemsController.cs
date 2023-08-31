using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using SagaPattern.Choreography.OrderApi.EF;
using SagaPattern.Choreography.OrderApi.Models;
using SagaPattern.Shared.Models;

namespace SagaPattern.Choreography.OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly OrderingContext _context;
        private readonly IPublisher _publisher;

        public OrderItemsController(OrderingContext context, IPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        // GET: api/OrderItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems()
        {
            return await _context.OrderItems.ToListAsync();
        }

        // GET: api/OrderItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItem>> GetOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);

            if (orderItem == null)
            {
                return NotFound();
            }

            return orderItem;
        }


        // POST: api/OrderItems
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task PostOrderItem(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            _publisher.Publish(JsonConvert.SerializeObject(new OrderRequest
                {
                    OrderId = orderItem.OrderId,
                    CatalogId = orderItem.ProductId,
                    Units = orderItem.Units,
                    Name = orderItem.ProductName
                }),
                "order_created_routingkey", // Routing key
                null);
        }
    }
}