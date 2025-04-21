using REIstacks.Domain.Events;

namespace REIstacks.Application.Common;
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
