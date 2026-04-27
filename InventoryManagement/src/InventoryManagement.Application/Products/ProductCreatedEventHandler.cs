using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace InventoryManagement.Products;

/* This is a Subscriber (Handler) example.
 * It implements IDistributedEventHandler<T> to listen for events
 * published via the Distributed Event Bus.
 */
public class ProductCreatedEventHandler 
    : IDistributedEventHandler<ProductCreatedEto>, ITransientDependency
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(ProductCreatedEto eventData)
    {
        // Example logic: Log the event or send a notification
        _logger.LogInformation(
            "----- New Product Created Event Received -----" +
            "\nProduct ID: {Id}" +
            "\nProduct Name: {Name}" +
            "\nCreated By: {CreatorId}",
            eventData.Id,
            eventData.Name,
            eventData.CreatorId
        );

        // You could also perform other side-effects here:
        // - Send an email to the admin
        // - Sync data with another system
        // - Update a search index (e.g. Elasticsearch)
        
        await Task.CompletedTask;
    }
}
