using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;

namespace Utils.Messaging;

public class MessageSender : IDisposable
{
    private static readonly ActivitySource ActivitySource = new(nameof(MessageSender));
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    private readonly ILogger<MessageSender> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageSender(ILogger<MessageSender> logger)
    {
        _logger = logger;
        _connection = RabbitMqHelper.CreateConnection();
        _channel = RabbitMqHelper.CreateModelAndDeclareTestQueue(_connection);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;

        _channel.Dispose();
        _connection.Dispose();
    }

    public string SendMessage()
    {
        try
        {
            var activityName = $"{RabbitMqHelper.TestQueueName} send";

            using var activity = ActivitySource.StartActivity(activityName, ActivityKind.Producer);
            var props = _channel.CreateBasicProperties();

            ActivityContext contextToInject = default;
            if (activity is not null)
                contextToInject = activity.Context;
            else if (Activity.Current is not null)
                contextToInject = Activity.Current.Context;

            Propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), props,
                InjectTraceContextIntoBasicProperties);

            RabbitMqHelper.AddMessagingTags(activity);
            var body = $"Published message: DateTimeOffset.UtcNow = {DateTimeOffset.UtcNow}.";

            _channel.BasicPublish(
                exchange: RabbitMqHelper.DefaultExchangeName,
                routingKey: RabbitMqHelper.TestQueueName,
                basicProperties: props,
                body: Encoding.UTF8.GetBytes(body));

            _logger.LogInformation("Message sent: [{Body}]", body);

            return body;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Message publishing failed.");
            throw;
        }
    }

    private void InjectTraceContextIntoBasicProperties(IBasicProperties props, string key, string value)
    {
        try
        {
            props.Headers ??= new Dictionary<string, object>();

            props.Headers[key] = value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to inject trace context.");
        }
    }

    ~MessageSender()
    {
        Dispose(false);
    }
}
