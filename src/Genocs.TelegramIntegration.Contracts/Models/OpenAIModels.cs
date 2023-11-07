using System.Text.Json.Serialization;

namespace Genocs.TelegramIntegration.Contracts.Models;

public class OpenAIRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "text-davinci-003";

    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.5;

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 60;

    [JsonPropertyName("top_p")]
    public int TopP { get; set; } = 1;

    [JsonPropertyName("frequency_penalty")]
    public double FrequencyPenalty { get; set; } = 0.5;

    [JsonPropertyName("presence_penalty")]
    public int PresencePenalty { get; set; }

    [JsonPropertyName("stop")]
    public List<string> Stop { get; set; } = new List<string> { "You:" };
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
