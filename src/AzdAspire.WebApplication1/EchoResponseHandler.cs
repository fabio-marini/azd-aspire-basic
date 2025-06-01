using AzdAspire.ServiceDefaults;
using Rebus.Handlers;
using Rebus.Pipeline;

/// <summary>
/// Handles <see cref="EchoResponse"/> messages by logging their content and relevant headers.
/// </summary>
public class EchoResponseHandler : IHandleMessages<EchoResponse>
{
    private readonly IMessageContext ctx;
    private readonly ILogger<EchoResponseHandler> log;

    /// <summary>
    /// Initializes a new instance of the <see cref="EchoResponseHandler"/> class.
    /// </summary>
    /// <param name="ctx">The message context providing access to message headers and other metadata.</param>
    /// <param name="log">The logger used for logging information about handled messages.</param>
    public EchoResponseHandler(IMessageContext ctx, ILogger<EchoResponseHandler> log)
    {
        this.ctx = ctx;
        this.log = log;
    }

    /// <summary>
    /// Handles the incoming <see cref="EchoResponse"/> message by logging its details.
    /// </summary>
    /// <param name="message">The <see cref="EchoResponse"/> message to handle.</param>
    /// <returns>A completed <see cref="Task"/>.</returns>
    public Task Handle(EchoResponse message)
    {
        log.LogInformation("Received {MessageType} with message: {EchoMessage} and headers {EchHeaders}",
            message.GetType().ToString(), message.Message, ctx.Headers.Where(h => h.Key.StartsWith("x-app")));

        return Task.CompletedTask;
    }
}
