using Microsoft.Extensions.DependencyInjection;
using REIstacks.Application.Common;
using REIstacks.Application.Interfaces.IEventHandlers;
using REIstacks.Domain.Events;

namespace REIstacks.Infrastructure.EventDispatching;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var eventType = domainEvent.GetType();
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod("HandleAsync");
            await (Task)method.Invoke(handler, new object[] { domainEvent, cancellationToken });
        }
    }
}