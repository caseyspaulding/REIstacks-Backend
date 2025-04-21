using REIstacks.Domain.Events;

namespace REIstacks.Application.Interfaces.IEventHandlers;

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}