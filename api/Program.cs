

using System.Reflection;
using System.Text.Json;
using Fleck;
using lib;
using ws;

public static class Startup
{
    public static void Main(string[] args)
    {
        var app = Statup(args);
        app.Run();
    }

    public static WebApplication Statup(string[] args)
    {
        Console.WriteLine(Environment.GetEnvironmentVariable("AZ"));
        var builder = WebApplication.CreateBuilder(args);

        var clientEventHandlers = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());

        var app = builder.Build();
        
        builder.WebHost.UseUrls($"http://*:9999");
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8181";
        var server = new WebSocketServer("ws://0.0.0.0:"+port);


        server.Start(ws =>
        {
            var keepAliveInterval = TimeSpan.FromSeconds(30); 
            var keepAliveTimer = new System.Timers.Timer(keepAliveInterval.TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };
            keepAliveTimer.Elapsed += (sender, e) => {
                try
                {
                    ws.Send("ping");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception in keep-alive timer: " + ex.Message);
                    keepAliveTimer.Stop(); 
                }
            };
            ws.OnClose = () =>
            {
                Console.WriteLine("Connection Closed");
                keepAliveTimer.Stop(); 
            };
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
                    ws.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClient()
                    {
                        errorMessage = e.Message
                    }));
                    
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.InnerException);
                    Console.WriteLine(e.StackTrace);
                    // your exception handling here
                }
            };
  
        });
        return app;

    }
}


public class ServerSendsErrorMessageToClient : BaseDto
{
    public string errorMessage { get; set; }
}





