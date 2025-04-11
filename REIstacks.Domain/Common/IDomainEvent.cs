namespace REIstacks.Domain.Common;

// No MediatR dependency
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}