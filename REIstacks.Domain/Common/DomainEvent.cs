﻿using REIstacks.Domain.Events;

namespace REIstacks.Domain.Common;

public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}