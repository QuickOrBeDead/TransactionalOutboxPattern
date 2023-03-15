namespace TransactionalOutboxPatternApp.Infrastructure.Service;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using TransactionalOutboxPatternApp.Infrastructure.Data;
using TransactionalOutboxPatternApp.Infrastructure.Entity;

public interface IEventLogService
{
    Task SaveEventLogEntryAsync(EventLogEntry eventLogEntry, IDbContextTransaction transaction);
}

public sealed class EventLogService : IEventLogService
{
    private readonly EventLogDbContext _eventLogDbContext;

    public EventLogService(EventLogDbContext eventLogDbContext)
    {
        _eventLogDbContext = eventLogDbContext ?? throw new ArgumentNullException(nameof(eventLogDbContext));
    }

    public Task SaveEventLogEntryAsync(EventLogEntry eventLogEntry, IDbContextTransaction transaction)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        _eventLogDbContext.Database.UseTransaction(transaction.GetDbTransaction());
        _eventLogDbContext.EventLogEntries.Add(eventLogEntry);

        return _eventLogDbContext.SaveChangesAsync();
    }
}