using System.Text.Json;
using Fleck;
using lib;

namespace ws;

public class ClientWantsToBroadcastToRoomDto : BaseDto
{
    public string message { get; set; }
    public int roomId { get; set; }
}

public class ClientWantsToBroadcastToRoom : BaseEventHandler<ClientWantsToBroadcastToRoomDto>
{
    public override Task Handle(ClientWantsToBroadcastToRoomDto dto, IWebSocketConnection socket)
    {
        var message = new ServerBroadcastsMessageWithUsername()
        {
            message = dto.message,
            username = StateService.Connections[socket.ConnectionInfo.Id].Username
        };
        StateService.BroadcastToRoom(dto.roomId, JsonSerializer.Serialize(
            message));
        return Task.CompletedTask;
    }
}

public class ServerBroadcastsMessageWithUsername : BaseDto
{
    public string message { get; set; }
    public string username { get; set; }
}