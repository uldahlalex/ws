using System.Text.Json;
using Fleck;
using lib;

namespace ws;

public class ClientWantsToSignInDto : BaseDto
{
    public string Username { get; set; }
}

public class ClientWantsToSignIn : BaseEventHandler<ClientWantsToSignInDto> 
{
    public override Task Handle(ClientWantsToSignInDto dto, IWebSocketConnection socket)
    {
        StateService.Connections[socket.ConnectionInfo.Id].Username = dto.Username;
        socket.Send(JsonSerializer.Serialize(new ServerWelcomesUser()));
        return Task.CompletedTask;
    }
}

public class ServerWelcomesUser : BaseDto;


