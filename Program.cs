// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add SignalR to the service container.
builder.Services.AddSignalR();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Map SignalR Hub.
app.MapHub<ChatHub>("/chathub");

app.Run();
