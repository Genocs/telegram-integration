using Genocs.Integration.CognitiveServices.IntegrationEvents;
using Genocs.Integration.CognitiveServices.Interfaces;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Contracts.Models;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Options;
using Genocs.TelegramIntegration.Services.Interfaces;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Web;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.Payments;

namespace Genocs.TelegramIntegration.Services;

public class TelegramProxy : ITelegramProxy
{
    private readonly ILogger<TelegramProxy> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMongoDbRepository<ChatUpdate> _chatUpdateRepository;
    private readonly TelegramSettings _telegramOptions;
    private readonly OpenAISettings _openAIOptions;
    private readonly ApiClientSettings _apiClientOptions;
    private readonly StripeSettings _stripeOptions;
    private readonly IFormRecognizer _formRecognizerService;
    private readonly IImageClassifier _formClassifierService;
    private readonly IDistributedCache _distributedCache;
    private readonly IPublishEndpoint _publishEndpoint;

    public TelegramProxy(
                         IOptions<TelegramSettings> telegramOptions,
                         ILogger<TelegramProxy> logger,
                         IOptions<OpenAISettings> openAIOptions,
                         IOptions<ApiClientSettings> apiClientOptions,
                         IOptions<StripeSettings> stripeOptions,
                         IHttpClientFactory httpClientFactory,
                         IMongoDbRepository<ChatUpdate> chatUpdateRepository,
                         IFormRecognizer formRecognizerService,
                         IImageClassifier formClassifierService,
                         IDistributedCache distributedCache,
                         IPublishEndpoint publishEndpoint)
    {
        if (telegramOptions == null) throw new ArgumentNullException(nameof(telegramOptions));
        if (telegramOptions.Value == null) throw new ArgumentNullException(nameof(telegramOptions.Value));

        _telegramOptions = telegramOptions.Value;

        if (openAIOptions == null) throw new ArgumentNullException(nameof(openAIOptions));
        if (openAIOptions.Value == null) throw new ArgumentNullException(nameof(openAIOptions.Value));

        _openAIOptions = openAIOptions.Value;

        if (apiClientOptions == null) throw new ArgumentNullException(nameof(apiClientOptions));
        if (apiClientOptions.Value == null) throw new ArgumentNullException(nameof(apiClientOptions.Value));

        _apiClientOptions = apiClientOptions.Value;

        if (stripeOptions == null) throw new ArgumentNullException(nameof(stripeOptions));
        if (stripeOptions.Value == null) throw new ArgumentNullException(nameof(stripeOptions.Value));

        _stripeOptions = stripeOptions.Value;

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

        _chatUpdateRepository = chatUpdateRepository ?? throw new ArgumentNullException(nameof(chatUpdateRepository));

        _formRecognizerService = formRecognizerService ?? throw new ArgumentNullException(nameof(formRecognizerService));
        _formClassifierService = formClassifierService ?? throw new ArgumentNullException(nameof(formClassifierService));
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    /// <summary>
    /// It Allows to pull the messages handled by the bot.
    /// </summary>
    public async Task PullUpdatesAsync()
    {
        // You need a BotClient instance if you want access to the Bot API methods.

        BotClient botClient = new BotClient(_telegramOptions.Token!);

        var updates = botClient.GetUpdates();

        if (updates is null || !updates.Any())
        {
            // No updates. Log and exit
            _logger.LogInformation("PullUpdatesAsync: No updates received");
            return;
        }

        // Process the updates received by context
        // Evaluate to use a queue to process the updates
        // or an actor pattern to process the updates

        // Check Texts
        var textToUpdates = updates.Where(c => c.Message != null && !string.IsNullOrWhiteSpace(c.Message.Text));

        if (textToUpdates != null && textToUpdates.Any())
        {
            foreach (var update in textToUpdates)
            {
                var exist = await _chatUpdateRepository.FirstOrDefaultAsync(c => c.Message.UpdateId == update.UpdateId);

                if (exist is null)
                {
                    var response = await CallGPTAsync(new OpenAIRequest { Prompt = update.Message.Text });

                    if (response != null && response.Choices?.Any() == true)
                    {
                        var choice = response.Choices.FirstOrDefault();
                        if (choice != null)
                        {
                            var res = botClient.SendMessage(update.Message.Chat.Id, choice.Text);
                        }
                    }
                }
            }
        }

        // Check Documents
        var documentToUpdates = updates.Where(c => c.Message != null && c.Message.Document != null);
        if (documentToUpdates != null && documentToUpdates.Any())
        {
            foreach (var update in documentToUpdates)
            {
                if (string.IsNullOrEmpty(update.Message?.Caption))
                {
                    var res = botClient.SendMessage(update.Message.Chat.Id, "Ciao, non capisco che cosa mi hai mandato!");
                    continue;
                }

                if (!string.IsNullOrEmpty(update.Message.Caption) && update.Message.Caption.ToLower().Contains("tff"))
                {
                    var res = botClient.SendMessage(update.Message.Chat.Id, "Ciao, ho ricevuto la tua fattura TaxFree e ho iniziato a processarla!");
                    continue;
                }

                // Cognitive services integration here
                string? fileId = update?.Message?.Photo?.OrderByDescending(c => c.FileSize).First().FileId;
                if (!string.IsNullOrEmpty(fileId))
                {
                    await ExtractSemanticDataAsync(fileId);
                }
            }
        }

        // Check Images
        var photoToUpdates = updates.Where(c => c.Message != null && c.Message.Photo != null);
        if (photoToUpdates != null && photoToUpdates.Any())
        {
            foreach (var update in photoToUpdates)
            {
                if (string.IsNullOrEmpty(update.Message?.Caption))
                {
                    var res = botClient.SendMessage(update.Message.Chat.Id, "Ciao, non capisco che cosa mi hai mandato!");
                    continue;
                }

                if (!string.IsNullOrEmpty(update.Message?.Caption) && update.Message.Caption.ToLower().Contains("tff"))
                {
                    var res = botClient.SendMessage(update.Message.Chat.Id, "Ciao, ho ricevuto la tua fattura TaxFree e ho iniziato a processarla!");
                    continue;
                }

                // Cognitive services integration here
                string? fileId = update?.Message?.Photo?.OrderByDescending(c => c.FileSize).First().FileId;
                if (!string.IsNullOrEmpty(fileId))
                {
                    await ExtractSemanticDataAsync(fileId);
                }
            }
        }

        // Check Invoice
        var invoiceToUpdates = updates.Where(c => c.PreCheckoutQuery != null);
        if (invoiceToUpdates != null && invoiceToUpdates.Any())
        {
            foreach (var update in invoiceToUpdates)
            {
                botClient.AnswerPreCheckoutQuery(update.PreCheckoutQuery.Id, true);
            }
        }
    }

    private async Task<OpenAIResponse?> CallGPTAsync(OpenAIRequest request)
    {
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _openAIOptions.Url)
        {
            Headers =
            {
                { HeaderNames.Accept, "application/json" },
                { HeaderNames.Authorization, $"Bearer {_openAIOptions.APIKey}"}
            },
            Content = JsonContent.Create(request)
        };

        using var httpClient = _httpClientFactory.CreateClient();
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<OpenAIResponse>(contentStream);
        }

        return null;
    }

    /// <summary>
    /// Process a message from Telegram.
    /// This function is called by the webhook.
    /// Pay attention: the order how the kind of messages are processed matters.
    /// </summary>
    /// <param name="message">The message received by the webhook.</param>
    /// <returns>async task.</returns>
    public async Task ProcessMessageAsync(Update? message)
    {
        if (message is null)
        {
            _logger.LogError("ProcessMessageAsync: Update message is null");
            return;
        }

        var messageToProcess = await _chatUpdateRepository.FirstOrDefaultAsync(c => c.Message.UpdateId == message.UpdateId);

        if (messageToProcess is null)
        {
            messageToProcess = new ChatUpdate(message);
            await _chatUpdateRepository.InsertAsync(messageToProcess);
        }
        else
        {
            if (messageToProcess.Processed)
            {
                _logger.LogError($"ProcessMessageAsync: received duplicated UpdateId: {message.UpdateId}");
                return;
            }
        }

        // Make payment unique
        // Payment
        if (message.PreCheckoutQuery != null)
        {
            BotClient botClient = new BotClient(_telegramOptions.Token!);
            botClient.AnswerPreCheckoutQuery(message.PreCheckoutQuery.Id, true);

            // Notify to the platform the payment is completed
            // Remember the process is handled by the Stripe. So you can find the payment into the Stripe dashboard
            // You can use the Stripe API to check the payment status or you can use the Stripe webhook to receive the payment status.

            // Send qr code
            var res = await botClient.SendPhotoAsync(
                                         chatId: message.PreCheckoutQuery.From.Id,
                                         photo: $"https://qrcode.tec-it.com/API/QRCode?data={message.PreCheckoutQuery.Id}",
                                         caption: $"This is Your Voucher, please use it during the checkout. It will expire on: {DateTime.UtcNow.AddDays(10).ToLongDateString()}!");

            return;
        }

        // Check language
        // message.Message.From.LanguageCode

        // Image
        if (message.Message.Photo != null)
        {
            // Use Semantic kernel to process the message
            await SendMessageAsync(message.Message.Chat.Id, $"Hello {message.Message.Chat.FirstName}, I got your image and I'm going to process it!");

            // Use the file Id to extract Semantic Data
            string fileId = message.Message.Photo.OrderByDescending(c => c.FileSize).First().FileId;

            var formRecognizerResponse = await CallFormRecognizerAsync(fileId);

            // var formRecognizerResponse = await ExtractSemanticDataAsync(fileId);

            string? processFormResponse = await ProcessFormResponseAsync(
                                                                         formRecognizerResponse,
                                                                         message.Message.Chat.Id,
                                                                         message.Message.Chat.FirstName);

            await Checkout(message.Message.Chat.Id, processFormResponse);
            return;
        }

        // Check empty message text
        if (string.IsNullOrWhiteSpace(message.Message.Text))
        {
            _logger.LogInformation($"ProcessMessageAsync received Message without text: {message.UpdateId}");
            return;
        }

        // Check message text commands
        if (message.Message.Text.Trim().StartsWith("/") || message.Message.Text.Trim().StartsWith("@"))
        {
            _logger.LogInformation($"ProcessMessageAsync received Message with command text: {message.UpdateId}");
            return;
        }

        // Check message text commands
        if (message.Message.Text.Trim().StartsWith("#"))
        {
            await SendMessageAsync(message.Message.From.Id, "Thanks for request a suggestion. Unfortunately Fiscanner integration is a work in progress!");
            _logger.LogInformation($"ProcessMessageAsync received Message with suggestion: {message.UpdateId}");
            return;
        }

        // Generic test call OpenAI with chat competition model
        var response = await CallGPTAsync(new OpenAIRequest { Prompt = message.Message.Text });

        if (response != null && response.Choices != null && response.Choices.Any())
        {
            var choice = response.Choices.FirstOrDefault();
            if (choice != null)
            {
                await SendMessageAsync(message.Message.From.Id, choice.Text);
            }
        }

        messageToProcess.Processed = true;
        await _chatUpdateRepository.UpdateAsync(messageToProcess);
    }

    /// <summary>
    /// Proxy to send a message to a recipient.
    /// </summary>
    /// <param name="recipient">The Recipient as chatId.</param>
    /// <param name="message">Payload with the message.</param>
    /// <returns>The async task.</returns>
    public async Task<Telegram.BotAPI.AvailableTypes.Message?> SendMessageAsync(long recipient, string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            _logger.LogError("SendMessageAsync: message is null or empty");
            return null;
        }

        BotClient botClient = new BotClient(_telegramOptions.Token!);
        return await botClient.SendMessageAsync(recipient, message);
    }

    private async Task<FormDataExtractionCompleted?> CallFormRecognizerAsync(string fileId)
    {
        try
        {
            BotClient botClient = new BotClient(_telegramOptions.Token!);

            var botFile = await botClient.GetFileAsync(fileId);
            if (botFile is null)
            {
                return null;
            }

            string urlResource = $"https://api.telegram.org/file/bot{_telegramOptions.Token}/{botFile.FilePath}";

            if (string.IsNullOrEmpty(urlResource))
            {
                return null;
            }

            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _apiClientOptions.FormRecognizerUrl)
            {
                Headers =
                {
                    { HeaderNames.Accept, "application/json" }
                },
                Content = JsonContent.Create(new { Url = urlResource })
            };

            using var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<FormDataExtractionCompleted?>(contentStream, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(500, ex, "ProcessMessageAsync exception while processing CallFormRecognizerAsync");
        }

        return null;
    }

    private async Task<FormDataExtractionCompleted?> ExtractSemanticDataAsync(string fileId)
    {
        FormDataExtractionCompleted result = new FormDataExtractionCompleted();

        try
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                _logger.LogWarning("CallCognitiveServicesAsync: fileId is null or empty");
                return null;
            }

            BotClient botClient = new BotClient(_telegramOptions.Token!);

            var botFile = await botClient.GetFileAsync(fileId);
            if (botFile is null)
            {
                _logger.LogWarning($"CallCognitiveServicesAsync: botFile is null. fileId: '{fileId}'");
                return null;
            }

            string urlResource = $"https://api.telegram.org/file/bot{_telegramOptions.Token}/{botFile.FilePath}";
            result.ResourceUrl = HttpUtility.HtmlDecode(urlResource);

            // Classify the image to identify the document issuer
            var classification = await _formClassifierService.ClassifyAsync(result.ResourceUrl);

            if (classification is null)
            {
                _logger.LogWarning($"CallCognitiveServicesAsync: classification is null. ResourceUrl: '{result.ResourceUrl}'");
                return null;
            }

            if (classification.Predictions is null || classification.Predictions.Count == 0)
            {
                _logger.LogWarning($"CallCognitiveServicesAsync: classification is empty. ResourceUrl: '{result.ResourceUrl}'");
                return null;
            }

            var firstPrediction = classification.Predictions.OrderByDescending(o => o.Probability).First();

            // This is a workaround to setup distributed cache
            await _distributedCache.SetAsync("d1fdb12d-c360-4e80-a7e8-75ff63971f0c", Encoding.UTF8.GetBytes("5d3c9567-c874-4bfa-95a1-35588c81be91"));

            result.ContentData = await _formRecognizerService.ScanAsync(firstPrediction.TagId!, result.ResourceUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(500, ex, "ProcessMessageAsync exception while processing CallFormRecognizerAsync");
        }

        return result;
    }

    private async Task<string?> ProcessFormResponseAsync(FormDataExtractionCompleted? formExtractorResponse, long chatId, string? user)
    {
        if (formExtractorResponse == null)
        {
            _logger.LogWarning("ProcessFormResponseAsync: formExtractorResponse is null");
            return null;
        }

        if (formExtractorResponse.ContentData == null)
        {
            _logger.LogWarning("ProcessFormResponseAsync: formExtractorResponse.ContentData is null");
            return null;
        }

        if (formExtractorResponse.ContentData.Count <= 0)
        {
            _logger.LogWarning("ProcessFormResponseAsync: formExtractorResponse.ContentData.Count <= 0");
            return null;
        }

        // Read from list of dynamic data contained into ContentData
        // Check the RefundAmount field inside the ContentData
        JsonElement dictionaryElement = (JsonElement)formExtractorResponse.ContentData[0];
        IDictionary<string, FormRecord?>? dictionaryData = dictionaryElement.Deserialize<IDictionary<string, FormRecord?>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (dictionaryData is null)
        {
            _logger.LogWarning("ProcessFormResponseAsync: dictionaryData is null");
            return null;
        }

        if (!dictionaryData.ContainsKey("RefundAmount"))
        {
            _logger.LogWarning("ProcessFormResponseAsync: dictionaryData doesn't contain RefundAmount");
            return null;
        }

        FormRecord? refund = JsonSerializer.Deserialize<FormRecord>(JsonSerializer.Serialize(dictionaryData["RefundAmount"]));

        if (refund is null)
        {
            _logger.LogWarning("ProcessFormResponseAsync: refund is null");
            return null;
        }

        // Save the document on the database for future reference

        /*
         *  MOVE OUTSIDE
         */

        // Use Telegram BOT client to send the voucher barcode image
        BotClient botClient = new BotClient(_telegramOptions.Token!);

        // Use Prompt engineering to setup response to the user

        await botClient.SendMessageAsync(chatId, $"Hello {user}, we received a TTF with the amount of {refund?.Value} EUR!");

        await botClient.SendMessageAsync(chatId, $"Hello {user}, you are eligible to receive a Voucher of 10 times {refund?.Value} EUR!");

        return refund?.Value;
    }

    private async Task Checkout(long chatId, string? amount)
    {
        // Convert Italian with decimal separator number to decimal

        IFormatProvider culture = new CultureInfo("it-IT", true);

        if (decimal.TryParse(amount, culture, out decimal value))
        {
            // Use Telegram BOT client to send start the payment checkout
            BotClient botClient = new BotClient(_telegramOptions.Token!);

            SendInvoiceArgs sendInvoiceArgs = new SendInvoiceArgs(
                                                                  chatId: chatId,
                                                                  title: "Genocs Voucher",
                                                                  description: $"Voucher of 10 times {amount} EUR!",
                                                                  payload: "GenocsVoucher",
                                                                  providerToken: _stripeOptions.Token!,
                                                                  currency: "EUR",
                                                                  prices: new List<LabeledPrice>
                                                                  {
                                                                  new LabeledPrice("Voucher", (int)(value * 1000))
                                                                  });

            await botClient.SendInvoiceAsync(sendInvoiceArgs);
        }

    }
}

public record FormRecord(float? Confidence, string? Value);