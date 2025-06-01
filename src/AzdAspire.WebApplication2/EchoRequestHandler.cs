using AzdAspire.ServiceDefaults;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Pipeline;

/// <summary>
/// Handles <see cref="EchoRequest"/> messages by logging the request, 
/// constructing an <see cref="EchoResponse"/>, and replying with application headers.
/// </summary>
public class EchoRequestHandler : IHandleMessages<EchoRequest>
{
    private readonly IBus bus;
    private readonly IMessageContext ctx;
    private readonly IConfiguration cfg;
    private readonly ILogger<EchoRequestHandler> log;

    /// <summary>
    /// Initializes a new instance of the <see cref="EchoRequestHandler"/> class.
    /// </summary>
    /// <param name="bus">The Rebus bus for sending replies.</param>
    /// <param name="ctx">The message context containing headers and metadata.</param>
    /// <param name="cfg">The application configuration provider.</param>
    /// <param name="log">The logger instance for logging information.</param>
    public EchoRequestHandler(IBus bus, IMessageContext ctx, IConfiguration cfg, ILogger<EchoRequestHandler> log)
    {
        this.bus = bus;
        this.ctx = ctx;
        this.cfg = cfg;
        this.log = log;
    }

    /// <summary>
    /// Handles the incoming <see cref="EchoRequest"/> message, logs its details,
    /// and replies with an <see cref="EchoResponse"/> and application headers.
    /// </summary>
    /// <param name="message">The <see cref="EchoRequest"/> message to handle.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
