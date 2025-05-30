using AzdAspire.ServiceDefaults;
using Microsoft.AspNetCore.Mvc;
using Rebus.Bus;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddAzureKeyVault();
builder.AddAzureAppConfiguration();
builder.AddRebus();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPost("/echo", async ([FromServices] IBus bus, [FromServices] IConfiguration cfg, EchoRequest req) =>
{
    var headers = new Dictionary<string, string>
    {
        { "x-appname", cfg.GetValue<string>("WebApp1:AppName") ?? "Not set! :(" },
        { "x-appkey", cfg.GetValue<string>("WebApp1:AppKey") ?? "Not set! :(" }
    };

    await bus.Send(req, headers);
});

app.Run();
