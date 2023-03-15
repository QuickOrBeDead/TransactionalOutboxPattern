namespace TransactionalOutboxPatternApp.Infrastructure.Service;

using Microsoft.EntityFrameworkCore;

using TransactionalOutboxPatternApp.Infrastructure.Data;
using TransactionalOutboxPatternApp.Infrastructure.Entity;

public interface IOrderService
{
    Task CreateOrderAsync();
}

public sealed class OrderService : IOrderService
{
    private readonly OrderDbContext _orderDbContext;

    public OrderService(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext ?? throw new ArgumentNullException(nameof(orderDbContext));
    }

    public async Task CreateOrderAsync()
    {
        var order = new Order { CreateDate = DateTime.Now };

        var strategy = _orderDbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteInTransactionAsync(
            _orderDbContext,
            operation: async (context, cancellationToken) =>
            {
                await context.Orders.AddAsync(order, cancellationToken).ConfigureAwait(false);
                await context.SaveChangesAsync(false, cancellationToken).ConfigureAwait(false);
            },
        verifySucceeded: (context, cancellationToken) => context.Orders.AsNoTracking().AnyAsync(b => b.Id == order.Id, cancellationToken: cancellationToken))
            .ConfigureAwait(false);

        _orderDbContext.ChangeTracker.AcceptAllChanges();
    }
}