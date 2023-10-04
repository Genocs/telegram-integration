using System.Text.Json.Serialization;

namespace Genocs.TelegramIntegration.Contracts.Models;

public class OpenAIRequest
{
    public string model { get; set; } = "text-davinci-003";
    public string? prompt { get; set; }
    public double temperature { get; set; } = 0.5;
    public int max_tokens { get; set; } = 60;
    public int top_p { get; set; } = 1;
    public double frequency_penalty { get; set; } = 0.5;
    public int presence_penalty { get; set; }
    public List<string> stop { get; set; } = new List<string> { "You:" };
}

public class Choice
{
    public string? text { get; set; }
    public int index { get; set; }
    public object? logprobs { get; set; }
    public string? finish_reason { get; set; }
}

public class OpenAIResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("object")]
    public string? ObjectId { get; set; }

    [JsonPropertyName("created")]
    public int Created { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("choices")]
    public List<Choice>? Choices { get; set; }

    [JsonPropertyName("usage")]
    public Usage? Usage { get; set; }
}

public class Usage
{
    public int prompt_tokens { get; set; }
    public int completion_tokens { get; set; }
    public int total_tokens { get; set; }
}
