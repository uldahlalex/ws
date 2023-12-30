

using Fleck;

var server = new WebSocketServer("ws://0.0.0.0:8181");

var wsConenctions = new List<IWebSocketConnection>();

server.Start(ws =>
{
    ws.OnOpen = () =>
    {
        wsConenctions.Add(ws);
    };
    ws.OnMessage = message =>
    {
        foreach (var webSocketConnection in wsConenctions)
        {
            webSocketConnection.Send(message);
        }
    };
});

WebApplication.CreateBuilder(args).Build().Run();
