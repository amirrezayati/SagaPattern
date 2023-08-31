using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using SagaPattern.Choreography.CatalogApi.EF;
using SagaPattern.Choreography.CatalogApi.Models;
using SagaPattern.Shared.Models;

namespace SagaPattern.Choreography.CatalogApi
{
    public class OrderCreatedListener : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly IPublisher _publisher;
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderCreatedListener(IServiceScopeFactory scopeFactory, IPublisher publisher, ISubscriber subscriber)
        {
            _scopeFactory = scopeFactory;
            _publisher = publisher;
            _subscriber = subscriber;
        }

        #region [- HostedService -]
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }
        #endregion

        #region [- Subscribe -]
        private bool Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<OrderRequest>(message);

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();
                try
                {
                    CatalogItem catalogItem = context.CatalogItems.Find(response.CatalogId) ?? new CatalogItem();

                    if (catalogItem is null || catalogItem.AvailableStock < response.Units)
                        throw new Exception();

                    catalogItem.AvailableStock -= response.Units;
                    context.Entry(catalogItem).State = EntityState.Modified;
                    context.SaveChanges();

                    _publisher.Publish(JsonConvert.SerializeObject(
                        new CatalogResponse { OrderId = response.OrderId, CatalogId = response.CatalogId, IsSuccess = true }
                    ), "catalog_response_routingkey", null);
                }
                catch (Exception)
                {
                    _publisher.Publish(JsonConvert.SerializeObject(
                        new CatalogResponse { OrderId = response.OrderId, CatalogId = response.CatalogId, IsSuccess = false }
                    ), "catalog_response_routingkey", null);
                }
            }

            return true;
        }
        #endregion
    }
}
