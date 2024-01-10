using Genocs.Integration.CognitiveServices.IntegrationEvents;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Options;
using Genocs.TelegramIntegration.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
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
    private readonly IOpenAIMiddleware _openAIMiddleware;
    private readonly ApiClientSettings _apiClientOptions;
    private readonly StripeSettings _stripeOptions;

    public TelegramProxy(
                         IOptions<TelegramSettings> telegramOptions,
                         ILogger<TelegramProxy> logger,
                         IOpenAIMiddleware openAIMiddleware,
                         IOptions<ApiClientSettings> apiClientOptions,
                         IOptions<StripeSettings> stripeOptions,
                         IHttpClientFactory httpClientFactory,
                         IMongoDbRepository<ChatUpdate> chatUpdateRepository)
    {
        if (telegramOptions == null) throw new ArgumentNullException(nameof(telegramOptions));
        if (telegramOptions.Value == null) throw new ArgumentNullException(nameof(telegramOptions.Value));

        _telegramOptions = telegramOptions.Value;
        _openAIMiddleware = openAIMiddleware;

        if (apiClientOptions == null) throw new ArgumentNullException(nameof(apiClientOptions));
        if (apiClientOptions.Value == null) throw new ArgumentNullException(nameof(apiClientOptions.Value));

        _apiClientOptions = apiClientOptions.Value;

        if (stripeOptions == null) throw new ArgumentNullException(nameof(stripeOptions));
        if (stripeOptions.Value == null) throw new ArgumentNullException(nameof(stripeOptions.Value));

        _stripeOptions = stripeOptions.Value;

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

        _chatUpdateRepository = chatUpdateRepository ?? throw new ArgumentNullException(nameof(chatUpdateRepository));
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
                    string? chatResponse = await _openAIMiddleware.ChatWithGPTAsync(update.Message.Text);
                    await SendMessageAsync(update.Message.Chat.Id, chatResponse);
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
                    // TODO: Extract Semantic Data
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
                    // TODO: Extract Semantic Data
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

        string? chatResponse;

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

            messageToProcess.Processed = true;
            await _chatUpdateRepository.UpdateAsync(messageToProcess);
            return;
        }

        // Check language
        // message.Message.From.LanguageCode

        // Image
        if (message?.Message?.Photo != null)
        {
            // Use Semantic kernel to process the message
            await SendMessageAsync(message.Message.Chat.Id, $"Hello {message.Message.Chat.FirstName}, I got your image and I'm going to process it!");

            // Use the file Id to extract Semantic Data
            string fileId = message.Message.Photo.OrderByDescending(c => c.FileSize).First().FileId;

            chatResponse = await CheckImageWithChatGPTAsync(fileId);
            await SendMessageAsync(message.Message.Chat.Id, chatResponse);

            await CallFormRecognizerAsync(fileId, message.UpdateId);
            messageToProcess.Processed = true;
            await _chatUpdateRepository.UpdateAsync(messageToProcess);
            return;
        }

        // Check empty message text
        if (string.IsNullOrWhiteSpace(message?.Message?.Text))
        {
            _logger.LogInformation($"ProcessMessageAsync received Message without text: {message?.UpdateId}");
            messageToProcess.Processed = true;
            await _chatUpdateRepository.UpdateAsync(messageToProcess);
            return;
        }

        // Check message text commands
        if (message.Message.Text.Trim().StartsWith("/") || message.Message.Text.Trim().StartsWith("@"))
        {
            _logger.LogInformation($"ProcessMessageAsync received Message with command text: {message.UpdateId}");
            messageToProcess.Processed = true;
            await _chatUpdateRepository.UpdateAsync(messageToProcess);
            return;
        }

        // Check message text commands
        if (message.Message.Text.Trim().StartsWith("#"))
        {
            await SendMessageAsync(message.Message.From.Id, "Thanks for request a suggestion. Unfortunately Fiscanner integration is a work in progress!");
            _logger.LogInformation($"ProcessMessageAsync received Message with suggestion: {message.UpdateId}");
            messageToProcess.Processed = true;
            await _chatUpdateRepository.UpdateAsync(messageToProcess);
            return;
        }

        chatResponse = await _openAIMiddleware.ChatWithGPTAsync(message.Message.Text);
        await SendMessageAsync(message.Message.From.Id, chatResponse);

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

    private async Task<FormDataExtractionCompleted?> CallFormRecognizerAsync(string fileId, int updateId)
    {
        try
        {
            BotClient botClient = new BotClient(_telegramOptions.Token!);

            var botFile = await botClient.GetFileAsync(fileId);
            if (botFile is null)
            {
                _logger.LogError("Resource: '{botFile}' is null or empty", botFile);
                return null;
            }

            string urlResource = $"https://api.telegram.org/file/bot{_telegramOptions.Token}/{botFile.FilePath}";

            if (string.IsNullOrEmpty(urlResource))
            {
                _logger.LogError("Resource: '{urlResource}' is null or empty", urlResource);
                return null;
            }

            // TODO: Refactor this code with Genocs HTTP Client
            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _apiClientOptions.FormRecognizerUrl)
            {
                Headers =
                {
                    { HeaderNames.Accept, "application/json" }
                },
                Content = JsonContent.Create(new
                {
                    RequestId = Guid.NewGuid().ToString(),
                    ContextId = Guid.NewGuid().ToString(),
                    ReferenceId = updateId.ToString(),
                    Url = urlResource,
                })
            };

            using var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                await using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<FormDataExtractionCompleted?>(contentStream, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            // End TODO
        }
        catch (Exception ex)
        {
            _logger.LogError(500, ex, "ProcessMessageAsync exception while processing CallFormRecognizerAsync");
        }

        return null;
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
        await botClient.SendMessageAsync(chatId, $"Hello {user}, your refund amount is {refund?.Value} EUR! Good news, you are eligible to receive a Voucher");
        return refund?.Value;
    }

    public async Task CheckoutAsync(long recipient, decimal amount, string currency = "EUR")
    {
        if (amount <= 0)
        {
            _logger.LogError("CheckoutAsync: amount cannot be less or equal to 0");
            return;
        }

        BotClient botClient = new BotClient(_telegramOptions.Token!);

        SendInvoiceArgs sendInvoiceArgs = new SendInvoiceArgs(
                                                              chatId: recipient,
                                                              title: "Genocs Voucher",
                                                              description: $"Voucher of {amount} EUR!",
                                                              payload: "GenocsVoucher",
                                                              providerToken: _stripeOptions.Token!,
                                                              currency: currency,
                                                              prices: new List<LabeledPrice>
                                                              {
                                                                  new LabeledPrice("Voucher", (int)(amount * 100))
                                                              });

        await botClient.SendInvoiceAsync(sendInvoiceArgs);
    }

    private async Task<string?> CheckImageWithChatGPTAsync(string? fileId)
    {
        string? response = null;

        if (string.IsNullOrEmpty(fileId))
        {
            _logger.LogError("called CheckImageWithChatGPTAsync. fileId cannot be null null or empty string");
            return response;
        }

        try
        {
            BotClient botClient = new BotClient(_telegramOptions.Token!);

            var botFile = await botClient.GetFileAsync(fileId);
            if (botFile is null)
            {
                return response;
            }

            string urlResource = $"https://api.telegram.org/file/bot{_telegramOptions.Token}/{botFile.FilePath}";

            if (string.IsNullOrEmpty(urlResource))
            {
                return response;
            }

            response = await _openAIMiddleware.ValidateTaxFreeFormAsync(urlResource);
        }
        catch (Exception ex)
        {
            _logger.LogError(500, ex, "ProcessMessageAsync exception while processing CallFormRecognizerAsync");
        }

        return response;
    }
}

public record FormRecord(float? Confidence, string? Value);