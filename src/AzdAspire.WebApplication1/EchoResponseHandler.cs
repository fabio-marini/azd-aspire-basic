using AzdAspire.ServiceDefaults;
using Rebus.Handlers;

public class EchoResponseHandler : IHandleMessages<EchoResponse>
{
    private readonly ILogger<EchoResponseHandler> log;

    public EchoResponseHandler(ILogger<EchoResponseHandler> log)
    {
        this.log = log;
    }

    public Task Handle(EchoResponse message)
    {
        log.LogInformation("Received {MessageType} with message: {EchoMessage}", message.GetType().ToString(), message.Message);

        return Task.CompletedTask;
    }
}
