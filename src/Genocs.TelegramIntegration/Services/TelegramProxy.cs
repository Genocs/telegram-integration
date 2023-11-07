using Genocs.Integration.CognitiveServices.Contracts;
using Genocs.Integration.CognitiveServices.Interfaces;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Contracts.Models;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Options;
using Genocs.TelegramIntegration.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
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
    private readonly IMongoDbRepository<GenocsChat> _mongoDbRepository;
    private readonly TelegramSettings _telegramOptions;
    private readonly OpenAISettings _openAIOptions;
    private readonly ApiClientSettings _apiClientOptions;
    private readonly StripeSettings _stripeOptions;
    private readonly IFormRecognizer _formRecognizerService;
    private readonly IImageClassifier _formClassifierService;
    private readonly IDistributedCache _distributedCache;

    public TelegramProxy(
                         IOptions<TelegramSettings> telegramOptions,
                         ILogger<TelegramProxy> logger,
                         IOptions<OpenAISettings> openAIOptions,
                         IOptions<ApiClientSettings> apiClientOptions,
                         IOptions<StripeSettings> stripeOptions,
                         IHttpClientFactory httpClientFactory,
                         IMongoDbRepository<GenocsChat> mongoDbRepository,
                         IFormRecognizer formRecognizerService,
                         IImageClassifier formClassifierService,
                         IDistributedCache distributedCache)
    {
        if (telegramOptions == null) throw new ArgumentNullException(nameof(telegramOptions));
        if (telegramOptions.Value == null) throw new ArgumentNullException(nameof(telegramOptions.Value));
        if (string.IsNullOrWhiteSpace(telegramOptions.Value.Token)) throw new ArgumentNullException("Token cannot be null");
        _telegramOptions = telegramOptions.Value;

        if (openAIOptions == null) throw new ArgumentNullException(nameof(openAIOptions));
        if (openAIOptions.Value == null) throw new ArgumentNullException(nameof(openAIOptions.Value));
        if (string.IsNullOrWhiteSpace(openAIOptions.Value.APIKey)) throw new ArgumentNullException("APIKey cannot be null");
        if (string.IsNullOrWhiteSpace(openAIOptions.Value.Url)) throw new ArgumentNullException("Url cannot be null");

        _openAIOptions = openAIOptions.Value;

        if (apiClientOptions == null) throw new ArgumentNullException(nameof(apiClientOptions));
        if (apiClientOptions.Value == null) throw new ArgumentNullException(nameof(apiClientOptions.Value));
        if (string.IsNullOrWhiteSpace(apiClientOptions.Value.FormRecognizerUrl)) throw new ArgumentNullException("FormRecognizerUrl cannot be null");

        _apiClientOptions = apiClientOptions.Value;

        if (stripeOptions == null) throw new ArgumentNullException(nameof(stripeOptions));
        if (stripeOptions.Value == null) throw new ArgumentNullException(nameof(stripeOptions.Value));
        if (string.IsNullOrWhiteSpace(stripeOptions.Value.Token)) throw new ArgumentNullException("Token cannot be null");
        _stripeOptions = stripeOptions.Value;

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _mongoDbRepository = mongoDbRepository ?? throw new ArgumentNullException(nameof(mongoDbRepository));

        _formRecognizerService = formRecognizerService ?? throw new ArgumentNullException(nameof(formRecognizerService));
        _formClassifierService = formClassifierService ?? throw new ArgumentNullException(nameof(formClassifierService));
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
    }

    /// <summary>
    /// It Allows to pull the messages handled by the bot.
    /// </summary>
    public async Task PullUpdatesAsync()
    {
        // You need a BotClient instance if you want access to the Bot API methods.

        BotClient botClient = new BotClient(_telegramOptions.Token);

        var updates = botClient.GetUpdates();

        if (updates?.Any() == true)
        {
            // Check Texts
            var textToUpdates = updates.Where(c => c.Message != null && !string.IsNullOrWhiteSpace(c.Message.Text));

            if (textToUpdates != null && textToUpdates.Any())
            {
                foreach (var update in textToUpdates)
                {
                    var exist = await _mongoDbRepository.FirstOrDefaultAsync(c => c.UpdateId == update.UpdateId);

                    if (exist is null)
                    {
                        GenocsChat genocsChat = new GenocsChat { UpdateId = update.UpdateId };
                        await _mongoDbRepository.InsertAsync(genocsChat);

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
                    var exist = await _mongoDbRepository.FirstOrDefaultAsync(c => c.UpdateId == update.UpdateId);
                    if (exist is null)
                    {
                        GenocsChat genocsChat = new GenocsChat { UpdateId = update.UpdateId };
                        await _mongoDbRepository.InsertAsync(genocsChat);

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
                        await CallCognitiveServicesAsync("");
                    }
                }
            }

            // Check Images
            var photoToUpdates = updates.Where(c => c.Message != null && c.Message.Photo != null);
            if (photoToUpdates != null && photoToUpdates.Any())
            {
                foreach (var update in photoToUpdates)
                {
                    var exist = await _mongoDbRepository.FirstOrDefaultAsync(c => c.UpdateId == update.UpdateId);
                    if (exist is null)
                    {
                        GenocsChat genocsChat = new GenocsChat { UpdateId = update.UpdateId };
                        await _mongoDbRepository.InsertAsync(genocsChat);

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

                        await CallCognitiveServicesAsync("");
                    }
                }
            }

            // Check Invoice
            var invoiceToUpdates = updates.Where(c => c.PreCheckoutQuery != null);
            if (invoiceToUpdates != null && invoiceToUpdates.Any())
            {
                foreach (var update in invoiceToUpdates)
                {
                    var exist = await _mongoDbRepository.FirstOrDefaultAsync(c => c.UpdateId == update.UpdateId);
                    if (exist is null)
                    {
                        GenocsChat genocsChat = new GenocsChat { UpdateId = update.UpdateId };
                        await _mongoDbRepository.InsertAsync(genocsChat);

                        botClient.AnswerPreCheckoutQuery(update.PreCheckoutQuery.Id, true);
                    }
                }
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

    public async Task ProcessMessageAsync(Update? message, string? rawData)
    {
        if (message is null)
        {
            _logger.LogError("ProcessMessageAsync: Update message is null");
            return;
        }

        // Payment
        if (message.PreCheckoutQuery != null)
        {
            BotClient botClient = new BotClient(_telegramOptions.Token);
            botClient.AnswerPreCheckoutQuery(message.PreCheckoutQuery.Id, true);

            // Check if qr code needs to be generated
            // Send qr code
            var res = await botClient.SendPhotoAsync(
                                         chatId: message.PreCheckoutQuery.From.Id,
                                         photo: $"https://qrcode.tec-it.com/API/QRCode?data={message.PreCheckoutQuery.Id}",
                                         caption: $"This is Your Voucher, please use it during the checkout. It will expire on: {DateTime.UtcNow.AddDays(10).ToLongDateString()}!");
            return;
        }

        if (message.Message is null)
        {
            _logger.LogError("ProcessMessageAsync: Update message.Message is null");
            return;
        }

        var exist = await _mongoDbRepository.FirstOrDefaultAsync(c => c.UpdateId == message.UpdateId);

        if (exist is not null)
        {
            _logger.LogError($"ProcessMessageAsync: received duplicated UpdateId: {message.UpdateId}");
            return;
        }

        GenocsChat chatMessage = new GenocsChat { UpdateId = message.UpdateId, Message = rawData };
        await _mongoDbRepository.InsertAsync(chatMessage);

        // Check language
        // message.Message.From.LanguageCode

        // Image
        if (message.Message.Photo != null)
        {
            await SendMessageAsync(message.Message.Chat.Id, $"Hello {message.Message.Chat.FirstName}, I got your image and I'm going to process it!");

            // Cognitive services integration here
            var formRecognizerResponse = await CallCognitiveServicesAsync(message.Message.Photo.OrderByDescending(c => c.FileSize).First().FileId);
            string? processFormResponse = await ProcessFormResponseAsync(formRecognizerResponse, message.Message.Chat.Id, message.Message.Chat.FirstName);
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

        // Generic test call GPT3
        var response = await CallGPTAsync(new OpenAIRequest { Prompt = message.Message.Text });

        if (response != null && response.Choices != null && response.Choices.Any())
        {
            var choice = response.Choices.FirstOrDefault();
            if (choice != null)
            {
                await SendMessageAsync(message.Message.From.Id, choice.Text);
            }
        }
    }

    public async Task SendMessageAsync(long recipient, string? message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            BotClient botClient = new BotClient(_telegramOptions.Token);
            var res = await botClient.SendMessageAsync(recipient, message);
        }
    }

    public async Task LogMessageAsync(string? message)
    {
        GenocsChat chatMessage = new GenocsChat { Message = message };
        await _mongoDbRepository.InsertAsync(chatMessage);
    }

    private async Task<OpenAIResponse?> CallFormRecognizerAsync(string fileId)
    {
        try
        {
            BotClient botClient = new BotClient(_telegramOptions.Token);

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
                return await JsonSerializer.DeserializeAsync<OpenAIResponse>(contentStream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(500, ex, "ProcessMessageAsync exception while processing CallFormRecognizerAsync");
        }

        return null;
    }

    private async Task<FormExtractorResponse?> CallCognitiveServicesAsync(string fileId)
    {
        FormExtractorResponse result = new FormExtractorResponse();

        try
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                return null;
            }

            BotClient botClient = new BotClient(_telegramOptions.Token);

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

            result.ResourceUrl = HttpUtility.HtmlDecode(urlResource);

            var classification = await _formClassifierService.ClassifyAsync(result.ResourceUrl);

            if (classification != null && classification.Predictions != null && classification.Predictions.Any())
            {
                var firstPrediction = classification.Predictions.OrderByDescending(o => o.Probability).First();
                await _distributedCache.SetAsync("d1fdb12d-c360-4e80-a7e8-75ff63971f0c", Encoding.UTF8.GetBytes("5d3c9567-c874-4bfa-95a1-35588c81be91"));
                result.ContentData = await _formRecognizerService.ScanAsync(firstPrediction.TagId!, urlResource);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(500, ex, "ProcessMessageAsync exception while processing CallFormRecognizerAsync");
        }

        return result;
    }

    private async Task<string?> ProcessFormResponseAsync(FormExtractorResponse? formExtractorResponse, long chatId, string? user)
    {
        if (formExtractorResponse == null)
        {
            return null;
        }

        if (formExtractorResponse.ContentData == null)
        {
            return null;
        }

        if (formExtractorResponse.ContentData.Count <= 0)
        {
            return null;
        }

        // Read from list of dynamic data contained into ContentData
        // Check the RefundAmount field inside the ContentData

        IDictionary<string, object?> dictionaryData = (System.Dynamic.ExpandoObject)formExtractorResponse.ContentData[0];

        if (dictionaryData is null)
        {
            return null;
        }

        if (!dictionaryData.ContainsKey("RefundAmount"))
        {
            return null;
        }

        FormRecord? refund = JsonSerializer.Deserialize<FormRecord>(JsonSerializer.Serialize(dictionaryData["RefundAmount"]));

        // Use Telegram BOT client to send the voucher barcode image
        BotClient botClient = new BotClient(_telegramOptions.Token);

        await botClient.SendMessageAsync(chatId, $"Hello {user}, we received a TTF with the amount of {refund?.Value} EUR!");

        await botClient.SendMessageAsync(chatId, $"Hello {user}, you are eligible to receive a Voucher of 10 times {refund?.Value} EUR!");

        return refund?.Value;
    }

    private async Task Checkout(long chatId, string? amount)
    {
        if (decimal.TryParse(amount, out decimal value))
        {
            // Use Telegram BOT client to send the voucher barcode image
            BotClient botClient = new BotClient(_telegramOptions.Token);

            SendInvoiceArgs sendInvoiceArgs = new SendInvoiceArgs(
                                                                  chatId: chatId,
                                                                  title: "Genocs Voucher",
                                                                  description: $"Voucher of 10 times {amount} EUR!",
                                                                  payload: "GenocsVoucher",
                                                                  providerToken: _stripeOptions.Token,
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