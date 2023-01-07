using System.Text.Json.Serialization;

namespace Genocs.TelegramIntegration.Contracts.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class TelegramMessage
    {
        [JsonPropertyName("update_id")]
        public int UpdateId { get; set; }

        [JsonPropertyName("message")]
        public Message? Message { get; set; }

        [JsonPropertyName("pre_checkout_query")]
        public PreCheckoutQuery? PreCheckoutQuery { get; set; }
    }

    public class Chat
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }

    public class From
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("is_bot")]
        public bool IsBot { get; set; }

        [JsonPropertyName("language_code")]
        public string? LanguageCode { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("message_id")]
        public int MessageId { get; set; }

        [JsonPropertyName("from")]
        public From? From { get; set; }

        [JsonPropertyName("chat")]
        public Chat? Chat { get; set; }

        [JsonPropertyName("date")]
        public int Date { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }


    public class PreCheckoutQuery
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("from")]
        public From? From { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("total_amount")]
        public int TotalAmount { get; set; }

        [JsonPropertyName("invoice_payload")]
        public string? InvoicePayload { get; set; }
    }
}
