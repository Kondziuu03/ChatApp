var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ChatApp_API>("apiservice-chat");
builder.AddProject<Projects.ChatApp_MessageBroker>("apiservice-message");

builder.Build().Run();
