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

app.MapPost("/echo", async ([FromServices] IBus bus, EchoRequest req) =>
{
    await bus.Send(req);
});

app.Run();
