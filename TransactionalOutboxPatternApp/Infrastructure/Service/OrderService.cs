namespace TransactionalOutboxPatternApp.Infrastructure.Service;

using Microsoft.EntityFrameworkCore;

using TransactionalOutboxPatternApp.Infrastructure.Data;
using TransactionalOutboxPatternApp.Infrastructure.Entity;
using TransactionalOutboxPatternApp.Infrastructure.Events;

public interface IOrderService
{
    Task<int> CreateOrderAsync();
}

public sealed class OrderService : IOrderService
{
    private readonly OrderDbContext _orderDbContext;

    private readonly IEventLogService _eventLogService;

    public OrderService(OrderDbContext orderDbContext, IEventLogService eventLogService)
    {
        _orderDbContext = orderDbContext ?? throw new ArgumentNullException(nameof(orderDbContext));
        _eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
    }

    public async Task<int> CreateOrderAsync()
    {
        var order = new Order { CreateDate = DateTime.Now };

        var strategy = _orderDbContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteInTransactionAsync(
            _orderDbContext,
            operation: async (context, cancellationToken) =>
            {
                await context.Orders.AddAsync(order, cancellationToken).ConfigureAwait(false);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                await _eventLogService.SaveEventAsync(new OrderCreatedEvent(order.Id));

                return order.Id;
            },
        verifySucceeded: (context, cancellationToken) => context.Orders.AsNoTracking().AnyAsync(b => b.Id == order.Id, cancellationToken: cancellationToken))
            .ConfigureAwait(false);
    }
}