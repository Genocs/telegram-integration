using Genocs.TelegramIntegration.Contracts.Models;
using Genocs.TelegramIntegration.Options;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Genocs.TelegramIntegration.Worker.HttpClients;

public class OpenAIService
{
    private readonly HttpClient _httpClient;
    public OpenAIService(IOptions<OpenAISettings> openAIOptions, HttpClient httpClient)
    {
        if (openAIOptions == null) throw new ArgumentNullException(nameof(openAIOptions));
        if (openAIOptions.Value == null) throw new ArgumentNullException(nameof(openAIOptions.Value));
        if (string.IsNullOrWhiteSpace(openAIOptions.Value.APIKey)) throw new ArgumentNullException("APIKey cannot be null");
        if (string.IsNullOrWhiteSpace(openAIOptions.Value.Url)) throw new ArgumentNullException("Url cannot be null");

        _httpClient = httpClient;

        _httpClient.BaseAddress = new Uri("https://api.openai.com");

        // using Microsoft.Net.Http.Headers;
        // The OpenAI requires two headers.
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {openAIOptions.Value.APIKey}");
    }

    public async Task<HttpResponseMessage> GetAspNetCoreDocsBranchesAsync(OpenAIRequest request)
    {
        using StringContent json = new(
            JsonSerializer.Serialize(request, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        return await _httpClient.PostAsync("v1/completions", json);

    }
}
