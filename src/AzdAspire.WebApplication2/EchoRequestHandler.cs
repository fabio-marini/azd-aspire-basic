using AzdAspire.ServiceDefaults;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Pipeline;

public class EchoRequestHandler : IHandleMessages<EchoRequest>
{
    private readonly IBus bus;
    private readonly IMessageContext ctx;
    private readonly IConfiguration cfg;
    private readonly ILogger<EchoRequestHandler> log;

    public EchoRequestHandler(IBus bus, IMessageContext ctx, IConfiguration cfg, ILogger<EchoRequestHandler> log)
    {
        this.bus = bus;
        this.ctx = ctx;
        this.cfg = cfg;
        this.log = log;
    }

    public async Task Handle(EchoRequest message)
    {
        log.LogInformation("Received {MessageType} with message: {EchoMessage} and headers {EchoHeaders}", 
            message.GetType().ToString(), message.Message, ctx.Headers.Where(h => h.Key.StartsWith("x-app")));

        var res = new EchoResponse(message.Message);

        var headers = new Dictionary<string, string>
        {
            { "x-appname", cfg.GetValue<string>("WebApp2:AppName") ?? "Not set! :(" },
            { "x-appkey", cfg.GetValue<string>("WebApp2:AppKey") ?? "Not set! :(" }
        };

        await bus.Reply(res, headers);
    }
}
