using AzdAspire.ServiceDefaults;
using Rebus.Handlers;
using Rebus.Pipeline;

public class EchoResponseHandler : IHandleMessages<EchoResponse>
{
    private readonly IMessageContext ctx;
    private readonly ILogger<EchoResponseHandler> log;

    public EchoResponseHandler(IMessageContext ctx, ILogger<EchoResponseHandler> log)
    {
        this.ctx = ctx;
        this.log = log;
    }

    public Task Handle(EchoResponse message)
    {
        log.LogInformation("Received {MessageType} with message: {EchoMessage} and headers {EchHeaders}",
            message.GetType().ToString(), message.Message, ctx.Headers.Where(h => h.Key.StartsWith("x-app")));

        return Task.CompletedTask;
    }
}
