using Domain.Services;
using Microsoft.AspNetCore.SignalR;
using Rdx.Serialization;
using RdxChat;
using RdxChat.Hubs;
using RdxChat.WebSocket;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<RdxSerializer>(_ => new RdxSerializer(new UserIdReplicaIdProvider()));
builder.Services.AddScoped<IWebSocketHandler, WebSocketHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<ChatHub>("/chatHub");
app.MapControllerRoute(
    "default",
    "{controller=MainMenu}/{action=Main}");

app.Run();