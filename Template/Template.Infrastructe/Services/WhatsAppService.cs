using System.Net.Http.Json;

public class WhatsAppService : IWhatsAppService
{
    private readonly HttpClient _httpClient;

    public WhatsAppService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> SendMessageAsync(string phone, string message)
    {
        var payload = new
        {
            phone,
            message
        };

        var response = await _httpClient.PostAsJsonAsync(
            "http://localhost:3000/send",
            payload
        );

        return response.IsSuccessStatusCode;
    }
}