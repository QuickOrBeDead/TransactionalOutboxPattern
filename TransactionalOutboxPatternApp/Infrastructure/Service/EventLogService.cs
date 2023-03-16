namespace TransactionalOutboxPatternApp.Infrastructure.Service;

using System.Text.Json;

using MessageQueue.Events;

using TransactionalOutboxPatternApp.Infrastructure.Data;
using TransactionalOutboxPatternApp.Infrastructure.Entity;

public interface IEventLogService
{
    Task SaveEventAsync(EventBase @event);
}

public sealed class EventLogService : IEventLogService
{
    private readonly OrderDbContext _orderDbContext;

    public EventLogService(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext ?? throw new ArgumentNullException(nameof(orderDbContext));
    }

    public Task SaveEventAsync(EventBase @event)
    {
        var eventLogEntry = new EventLogEntry
                                {
                                    EventId = @event.Id,
                                    CreateDate = @event.CreateDate,
                                    Content = JsonSerializer.Serialize(@event, @event.GetType(), new JsonSerializerOptions { WriteIndented = false }),
                                    EventTypeName = @event.GetType().FullName,
                                    State = EventLogEntryState.NotPublished
                                };
        _orderDbContext.EventLogEntries.Add(eventLogEntry);

        return _orderDbContext.SaveChangesAsync();
    }
}