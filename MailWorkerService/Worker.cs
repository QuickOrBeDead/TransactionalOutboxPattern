namespace MailWorkerService;

using MailWorkerService.Infrastructure.Events;

using MessageQueue;
using System.Net.Mail;
using System.Net;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IMessageQueueConsumerService<OrderCreatedEvent> _messageQueueConsumerService;

    public Worker(ILogger<Worker> logger, IMessageQueueConsumerService<OrderCreatedEvent> messageQueueConsumerService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageQueueConsumerService = messageQueueConsumerService ?? throw new ArgumentNullException(nameof(messageQueueConsumerService));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            _messageQueueConsumerService.ConsumeMessage(
                (m, t) =>
                    {
                        var message = new MailMessage();
                        var smtp = new SmtpClient();
                        message.From = new MailAddress("from@test.com");
                        message.To.Add(new MailAddress("to@test.com"));
                        message.Subject = $"Order {m.OrderId} is created";
                        message.IsBodyHtml = true;
                        message.Body = $"Hi,<br>Order {m.OrderId} is created at {m.CreateDate}";
                        smtp.Port = 25;
                        smtp.Host = "host.docker.internal";
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(message);

                        return true;
                    });
        }

        return Task.CompletedTask;
    }
}