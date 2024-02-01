

using System.Reflection;
using Fleck;
using lib;

var builder = WebApplication.CreateBuilder(args);

var clientEventHandlers = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());

var app = builder.Build();

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
        // evaluate whether or not message.eventType == 
            // trigger event handler
        try
        {
            app.InvokeClientEventHandler(clientEventHandlers, ws, message);

        }
        catch (Exception e)
        {
            // your exception handling here
        }
    };
});

Console.ReadLine();












