using System.Text.Json;
using Fleck;
using lib;

namespace ws;

public class ClientWantsToEnterRoomDto : BaseDto
{
    public int roomId { get; set; }
}

public class ClientWantsToEnterRoom : BaseEventHandler<ClientWantsToEnterRoomDto>
{
    public override Task Handle(ClientWantsToEnterRoomDto dto, IWebSocketConnection socket)
    {
        var isSuccess = StateService.AddToRoom(socket, dto.roomId);
        socket.Send(JsonSerializer.Serialize(new ServerAddsClientToRoom()
        {
            message = "you were succesfully added to room with ID " + dto.roomId
        }));
        return Task.CompletedTask;
    }
}

public class ServerAddsClientToRoom : BaseDto
{
    public string message { get; set; }
}







