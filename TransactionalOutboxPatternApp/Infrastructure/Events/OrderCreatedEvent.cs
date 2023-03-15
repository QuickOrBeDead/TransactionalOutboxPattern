namespace TransactionalOutboxPatternApp.Infrastructure.Events;

public class OrderCreatedEvent : EventBase
{
    public int OrderId { get; }

    public OrderCreatedEvent(int orderId)
    {
        OrderId = orderId;
    }
}