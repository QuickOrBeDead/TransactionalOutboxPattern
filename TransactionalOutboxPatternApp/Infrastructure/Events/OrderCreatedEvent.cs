namespace TransactionalOutboxPatternApp.Infrastructure.Events;

using MessageQueue.Events;

public class OrderCreatedEvent : EventBase
{
    public int OrderId { get; }

    public OrderCreatedEvent(int orderId)
    {
        OrderId = orderId;
    }
}