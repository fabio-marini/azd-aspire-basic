var builder = DistributedApplication.CreateBuilder(args);

// TODO: if Development only add rmq and emulator
// TODO: if Hybrid add akv, app config, asb and storage
// TODO: if Production same as Hybrid + container apps
// TODO: runtime vs publish?

builder.AddProject<Projects.AzdAspire_WebApplication1>("azdaspire-webapplication1");

builder.AddProject<Projects.AzdAspire_WebApplication2>("azdaspire-webapplication2");

builder.Build().Run();
