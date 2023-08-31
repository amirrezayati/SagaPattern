using Newtonsoft.Json;
using Plain.RabbitMQ;
using SagaPattern.Choreography.OrderApi.EF;
using SagaPattern.Shared.Models;

namespace SagaPattern.Choreography.OrderApi;

public class CatalogResponseListener : IHostedService
{
    private readonly ISubscriber _subscriber;
    private readonly IServiceScopeFactory _scopeFactory;

    public CatalogResponseListener(ISubscriber subscriber, IServiceScopeFactory scopeFactory)
    {
        _subscriber = subscriber;
        _scopeFactory = scopeFactory;
    }

    #region [- HostedService -]
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _subscriber.Subscribe(Subscribe);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    #endregion

    #region [- Subscribe -]
    private bool Subscribe(string message, IDictionary<string, object> header)
    {
        var response = JsonConvert.DeserializeObject<CatalogResponse>(message) ?? throw new ArgumentNullException();

        if (response.IsSuccess) return true;

        using (var scope = _scopeFactory.CreateScope())
        {
            var orderingContext = scope.ServiceProvider.GetRequiredService<OrderingContext>();

            // If transaction is not successful, Remove ordering item
            var orderItem = orderingContext.OrderItems.FirstOrDefault(o => o.ProductId == response.CatalogId && o.OrderId == response.OrderId);
            orderingContext.OrderItems.Remove(orderItem);
            orderingContext.SaveChanges();
        }
        return true;
    } 
    #endregion

}