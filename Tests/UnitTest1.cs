using lib;
using ws;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        Startup.Statup(null);
    }

    [Test]
    public async Task Test1()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        var ws2 = await new WebSocketTestClient().ConnectAsync();
        await ws.DoAndAssert(new ClientWantsToSignInDto()
        {
            Username = "Bob"
        }, r => r.Count(dto => dto.eventType == nameof(ServerWelcomesUser)) == 1);
        await ws2.DoAndAssert(new ClientWantsToSignInDto()
        {
            Username = "Alice"
        },r => r.Count(dto => dto.eventType == nameof(ServerWelcomesUser)) == 1);


        await ws.DoAndAssert(new ClientWantsToEnterRoomDto()
        {
            roomId = 1
        },r => r.Count(dto => dto.eventType == nameof(ServerAddsClientToRoom)) == 1);
        await ws2.DoAndAssert(new ClientWantsToEnterRoomDto()
        {
            roomId = 1
        },r => r.Count(dto => dto.eventType == nameof(ServerAddsClientToRoom)) == 1);

        await ws.DoAndAssert(new ClientWantsToBroadcastToRoomDto()
        {
            roomId = 1,
            message = "hey alice"
        },r => r.Count(dto => dto.eventType == nameof(ServerBroadcastsMessageWithUsername)) == 1);
        await ws2.DoAndAssert(new ClientWantsToBroadcastToRoomDto()
        {
            roomId = 1,
            message = "hey Bob"
        },r => r.Count(dto => dto.eventType == nameof(ServerBroadcastsMessageWithUsername)) == 2);
    }
}









