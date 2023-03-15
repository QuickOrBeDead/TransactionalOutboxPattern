namespace TransactionalOutboxPatternApp.Infrastructure.Events;

public class EventBase
{
    public Guid Id { get; }

    public DateTime CreateDate { get; }

    public EventBase()
    {
        Id = Guid.NewGuid();
        CreateDate = DateTime.Now;
    }
}