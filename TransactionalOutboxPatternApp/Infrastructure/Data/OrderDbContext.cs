namespace TransactionalOutboxPatternApp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

using TransactionalOutboxPatternApp.Infrastructure.Entity;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(
            builder =>
                {
                    builder.ToTable("Order");
                    builder.HasKey(e => e.Id);
                    builder.Property(e => e.Id).UseIdentityColumn();
                    builder.Property(e => e.CreateDate).IsRequired();
                });
    }
}