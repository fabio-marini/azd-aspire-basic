using AzdAspire.ServiceDefaults;
using Rebus.Bus;
using Rebus.Handlers;

public class EchoRequestHandler : IHandleMessages<EchoRequest>
{
    private readonly IBus bus;
    private readonly ILogger<EchoRequestHandler> log;

    public EchoRequestHandler(IBus bus, ILogger<EchoRequestHandler> log)
    {
        this.bus = bus;
        this.log = log;
    }

    public async Task Handle(EchoRequest message)
    {
        log.LogInformation("Received {MessageType} with message: {EchoMessage}", message.GetType().ToString(), message.Message);

        var res = new EchoResponse(message.Message);

        await bus.Reply(res);
    }
}
