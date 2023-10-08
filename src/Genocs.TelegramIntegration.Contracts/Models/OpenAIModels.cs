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
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("logprobs")]
    public object? LogProbs { get; set; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
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
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}
