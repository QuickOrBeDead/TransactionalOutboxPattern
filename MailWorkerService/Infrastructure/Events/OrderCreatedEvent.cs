namespace MailWorkerService.Infrastructure.Events;

using MessageQueue.Events;

[EventName("Order.OrderCreated")]
public sealed class OrderCreatedEvent : EventBase
{
    public int OrderId { get; }

    public OrderCreatedEvent()
    {
    }

    public OrderCreatedEvent(int orderId)
    {
        OrderId = orderId;
    }
}