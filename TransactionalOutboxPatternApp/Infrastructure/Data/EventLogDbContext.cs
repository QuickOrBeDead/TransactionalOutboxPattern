namespace TransactionalOutboxPatternApp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TransactionalOutboxPatternApp.Infrastructure.Entity;

public class EventLogDbContext : DbContext
{
    public EventLogDbContext(DbContextOptions<EventLogDbContext> options) : base(options)
    {
    }

    public DbSet<EventLogEntry> EventLogEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventLogEntry>(ConfigureIntegrationEventLogEntry);
    }

    private void ConfigureIntegrationEventLogEntry(EntityTypeBuilder<EventLogEntry> builder)
    {
        builder.ToTable("EventLogEntry");

        builder.HasKey(e => e.EventId);

        builder.Property(e => e.EventId)
            .IsRequired();

        builder.Property(e => e.Content)
            .IsRequired();

        builder.Property(e => e.CreateDate)
            .IsRequired();

        builder.Property(e => e.State)
            .IsRequired();

        builder.Property(e => e.TimesSent)
            .IsRequired();

        builder.Property(e => e.EventTypeName)
            .IsRequired();
    }
}