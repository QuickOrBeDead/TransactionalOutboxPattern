namespace TransactionalOutboxPatternApp.Infrastructure.Service;

using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using TransactionalOutboxPatternApp.Infrastructure.Data;
using TransactionalOutboxPatternApp.Infrastructure.Entity;
using TransactionalOutboxPatternApp.Infrastructure.Events;

public interface IEventLogService
{
    Task SaveEventAsync(EventBase @event, IDbContextTransaction transaction);
}

public sealed class EventLogService : IEventLogService
{
    private readonly EventLogDbContext _eventLogDbContext;

    public EventLogService(EventLogDbContext eventLogDbContext)
    {
        _eventLogDbContext = eventLogDbContext ?? throw new ArgumentNullException(nameof(eventLogDbContext));
    }

    public Task SaveEventAsync(EventBase @event, IDbContextTransaction transaction)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        _eventLogDbContext.Database.UseTransaction(transaction.GetDbTransaction());
       
        var eventLogEntry = new EventLogEntry
                                {
                                    EventId = @event.Id,
                                    CreateDate = @event.CreateDate,
                                    Content = JsonSerializer.Serialize(@event, @event.GetType(), new JsonSerializerOptions { WriteIndented = false }),
                                    State = EventLogEntryState.NotPublished
                                };
        _eventLogDbContext.EventLogEntries.Add(eventLogEntry);

        return _eventLogDbContext.SaveChangesAsync();
    }
}