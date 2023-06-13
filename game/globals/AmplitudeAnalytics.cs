using Godot;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;

public partial class AmplitudeAnalytics : Node
{
    private const string AmplitudeApiKey = "YOUR-KEY-HERE";

    public async void SendAmplitudeEvent(string eventType, string userId)
    {
        // Create an HttpClient instance
        using (var httpClient = new System.Net.Http.HttpClient())
        {
            // Create the Amplitude payload with the provided event type
            var payload = CreateAmplitudePayload(eventType, userId);

            // Create the Amplitude request with the created payload
            var request = CreateAmplitudeRequest(payload);

            // Send the request and process the response
            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                GD.Print("Amplitude event sent successfully: " + responseBody);
            }
            catch (HttpRequestException e)
            {
                GD.PrintErr("Error sending Amplitude event: " + e.Message);
            }
        }
    }

    private static HttpRequestMessage CreateAmplitudeRequest(string payload)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api2.amplitude.com/2/httpapi");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
        return request;
    }

    private string CreateAmplitudePayload(string eventType, string userId)
    {
        // Create the event data dictionary
        var eventData = new Dictionary<string, object>
    {
        { "event_type", eventType },
        { "user_id", userId }
    };

        // Create the payload dictionary
        var payload = new Dictionary<string, object>
    {
        { "api_key", AmplitudeApiKey },
        { "events", new List<object> { eventData } }
    };

        return JsonConvert.SerializeObject(payload);
    }
}
