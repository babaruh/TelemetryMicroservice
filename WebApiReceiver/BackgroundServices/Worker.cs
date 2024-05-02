using Utils.Messaging;

namespace WebApiReceiver.BackgroundServices;

public class Worker(MessageReceiver messageReceiver) : BackgroundService
{
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        messageReceiver.StartConsumer();

        await Task.CompletedTask.ConfigureAwait(false);
    }
}
