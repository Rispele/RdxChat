namespace RdxChat.WebSocket;

public interface IWebSocketHandler
{
    Task ConnectAsync();

    Task SendMessage(string message);
}