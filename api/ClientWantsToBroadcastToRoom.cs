using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Fleck;
using lib;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace ws;

public class ClientWantsToBroadcastToRoomDto : BaseDto
{
    public string message { get; set; }
    public int roomId { get; set; }
}

public class ClientWantsToBroadcastToRoom : BaseEventHandler<ClientWantsToBroadcastToRoomDto>
{
    public override async Task Handle(ClientWantsToBroadcastToRoomDto dto, IWebSocketConnection socket)
    {
        await isMessageToxic(dto.message);
        var message = new ServerBroadcastsMessageWithUsername()
        {
            message = dto.message,
            username = StateService.Connections[socket.ConnectionInfo.Id].Username
        };
        StateService.BroadcastToRoom(dto.roomId, JsonSerializer.Serialize(
            message));
    }

    public record RequestModel(string text, List<string> categories, string outputType)
    {
        public override string ToString()
        {
            return $"{{ text = {text}, categories = {categories}, outputType = {outputType} }}";
        }
    }

    private async Task isMessageToxic(string message)
    {
        HttpClient client = new HttpClient();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://toxicityfilter.cognitiveservices.azure.com/contentsafety/text:analyze?api-version=2023-10-01");

        request.Headers.Add("accept", "application/json");
        //The key from below has been regenerated / rotated on azure so it cannot be used - insert your own value (with environment variable of course)
        //I specifically do not want to change the code from I produced in the video, so I just leave this comment instead
        request.Headers.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("AZ"));

        var req = new RequestModel(message, new List<string>() { "Hate", "Violence" }, "FourSeverityLevels");

        request.Content = new StringContent(JsonSerializer.Serialize(req));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        var obj = JsonSerializer.Deserialize<ContentFilterResponse>(responseBody);
        var isToxic = obj.categoriesAnalysis.Count(e => e.severity > 1) >= 1;
        if (isToxic)
            throw new ValidationException(" Such speech is not allowed!");
    }
    
}

public class ServerBroadcastsMessageWithUsername : BaseDto
{
    public string message { get; set; }
    public string username { get; set; }
}

public class CategoriesAnalysis
{
    public string category { get; set; }
    public int severity { get; set; }
}

public class ContentFilterResponse
{
    public List<object> blocklistsMatch { get; set; }
    public List<CategoriesAnalysis> categoriesAnalysis { get; set; }
}

