using System.Text.Json.Serialization;

namespace Genocs.TelegramIntegration.Contracts.Models;

public class OpenAIRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "text-davinci-003";

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.5;

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 60;
}

public class PromptRequest : OpenAIRequest
{
    [JsonPropertyName("top_p")]
    public int TopP { get; set; } = 1;

    [JsonPropertyName("frequency_penalty")]
    public double FrequencyPenalty { get; set; } = 0.5;

    [JsonPropertyName("presence_penalty")]
    public int PresencePenalty { get; set; }

    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }

    [JsonPropertyName("stop")]
    public List<string> Stop { get; set; } = new List<string> { "You:" };
}

public class ImageRequest : OpenAIRequest
{
    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; } = new List<Message>();
}

public class Choice
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("logprobs")]
    public object? LogProbs { get; set; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }

    [JsonPropertyName("message")]
    public ResponseMessage? Message { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }
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

public class Content
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }
}

public class Message
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("content")]
    public List<Content>? Content { get; set; }
}

public class ResponseMessage
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }
}