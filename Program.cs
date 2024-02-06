

using System.Reflection;
using Fleck;
using lib;
using ws;

var builder = WebApplication.CreateBuilder(args);

var clientEventHandlers = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());

var app = builder.Build();

var server = new WebSocketServer("ws://0.0.0.0:8181");


server.Start(ws =>
{
    ws.OnOpen = () =>
    {
        StateService.AddConnection(ws);
    };
    ws.OnMessage = async message =>
    {
        // evaluate whether or not message.eventType == 
            // trigger event handler
        try
        {
            await app.InvokeClientEventHandler(clientEventHandlers, ws, message);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.InnerException);
            Console.WriteLine(e.StackTrace);
            // your exception handling here
        }
    };
});

Console.ReadLine();












