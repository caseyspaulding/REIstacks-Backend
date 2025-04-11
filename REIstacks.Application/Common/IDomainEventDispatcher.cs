using REIstacks.Domain.Common;

namespace REIstacks.Application.Common;
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
