using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Contracts.Models;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Options;
using Genocs.TelegramIntegration.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
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
    private readonly IMongoDbRepository<GenocsChat> _mongoDbRepository;
    private readonly TelegramSettings _telegramOptions;
    private readonly OpenAISettings _openAIOptions;

    private readonly List<string> _stopTags = new List<string> { @"/start" };

    public TelegramProxy(IOptions<TelegramSettings> telegramOptions,
        ILogger<TelegramProxy> logger,
        IOptions<OpenAISettings> openAIOptions,
        IHttpClientFactory httpClientFactory,
        IMongoDbRepository<GenocsChat> mongoDbRepository)
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

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _mongoDbRepository = mongoDbRepository ?? throw new ArgumentNullException(nameof(mongoDbRepository));
    }

    /// <summary>
    /// It Allows to pull the messages handled by the bot
    /// </summary>
    public async Task PullUpdatesAsync()
    {
        // You need a BotClient instance if you want access to the Bot API methods.

        BotClient botClient = new BotClient(_telegramOptions.Token);

        var updates = botClient.GetUpdates();

        if (updates != null && updates.Any())
        {
            // Check Texts
            var textToUpdates = updates.Where(c => c.Message != null && !string.IsNullOrWhiteSpace(c.Message.Text));

            if (textToUpdates != null && textToUpdates.Any())
            {
                foreach (var update in textToUpdates)
                {
                    var exist = await _mongoDbRepository.FirstOrDefaultAsync(c => c.UpdateId == update.UpdateId);

                    if (exist == null)
                    {
                        GenocsChat genocsChat = new GenocsChat { UpdateId = update.UpdateId };
                        await _mongoDbRepository.InsertAsync(genocsChat);

                        var response = await CallGPT3Async(new OpenAIRequest { prompt = update.Message.Text });

                        if (response != null && response.Choices != null && response.Choices.Any())
                        {
                            var choice = response.Choices.FirstOrDefault();
                            if (choice != null)
                            {
                                var res = botClient.SendMessage(update.Message.Chat.Id, choice.text);
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
                    if (exist == null)
                    {
                        GenocsChat genocsChat = new GenocsChat { UpdateId = update.UpdateId };
                        await _mongoDbRepository.InsertAsync(genocsChat);

                        if (string.IsNullOrEmpty(update.Message.Caption))
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
                    if (exist == null)
                    {
                        GenocsChat genocsChat = new GenocsChat { UpdateId = update.UpdateId };
                        await _mongoDbRepository.InsertAsync(genocsChat);

                        if (string.IsNullOrEmpty(update.Message.Caption))
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
                    if (exist == null)
                    {
                        GenocsChat genocsChat = new GenocsChat { UpdateId = update.UpdateId };
                        await _mongoDbRepository.InsertAsync(genocsChat);

                        botClient.AnswerPreCheckoutQuery(update.PreCheckoutQuery.Id, true);
                    }
                }
            }
        }
    }

    public bool CanCallGPT3(string textMessage)
    {
        if (string.IsNullOrWhiteSpace(textMessage)) return false;
        if (_stopTags.Contains(textMessage)) return false;

        return true;

    }

    public async Task<OpenAIResponse?> CallGPT3Async(OpenAIRequest request)
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


    public async Task<FormRecognizerResponse?> CallCognitiveServices(string resourceUrl)
    {

        FormRecognizerRequest request = new FormRecognizerRequest { Url = resourceUrl };
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "ScanForm/ClassifyAndEvaluate")
        {
            Content = JsonContent.Create(request)
        };

        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://genocs-form-extractor.azurewebsites.net/");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<FormRecognizerResponse>(content, options);
        }
        return null;
    }

    public async Task ProcessMessageAsync(Update message)
    {
        if (message == null)
        {
            _logger.LogError("ProcessMessageAsync received null message");
            return;
        }

        if (message.Message == null)
        {
            _logger.LogError("ProcessMessageAsync received null message.Message");
            return;
        }

        var exist = await _mongoDbRepository.FirstOrDefaultAsync(c => c.UpdateId == message.UpdateId);

        if (exist != null)
        {
            _logger.LogError($"ProcessMessageAsync received UpdateId that is still there: {message.UpdateId}");
            return;
        }

        GenocsChat chatMessage = new GenocsChat { UpdateId = message.UpdateId };
        await _mongoDbRepository.InsertAsync(chatMessage);

        // Payment
        if (message.PreCheckoutQuery != null)
        {
            BotClient botClient = new BotClient(_telegramOptions.Token);
            botClient.AnswerPreCheckoutQuery(message.PreCheckoutQuery.Id, true);
        }

        // Message
        if (message.Message == null)
        {
            return;
        }

        if (message.Message.Photo != null)
        {
            BotClient botClient = new BotClient(_telegramOptions.Token);

            if (string.IsNullOrEmpty(message.Message.Caption))
            {
                var res = botClient.SendMessage(message.Message.Chat.Id, "Ciao, non capisco che cosa mi hai mandato!");
            }

            if (!string.IsNullOrEmpty(message.Message.Caption) && message.Message.Caption.ToLower().Contains("tff"))
            {
                var res = botClient.SendMessage(message.Message.Chat.Id, "Ciao, ho ricevuto la tua fattura TaxFree e ho iniziato a processarla!");

                var photo = message.Message.Photo.OrderByDescending(o => o.FileSize).FirstOrDefault();

                if (photo != null)
                {
                    try
                    {
                        // Ask telegram foto url
                        var telegramFile = botClient.GetFile(photo.FileId);

                        // Build the full url  
                        string resourceUrl = $"https://api.telegram.org/file/bot{_telegramOptions.Token}/{telegramFile.FilePath}";

                        // Call form Recognizer
                        var cognitiveServicesResponse = await CallCognitiveServices(resourceUrl);

                        if (cognitiveServicesResponse != null)
                        {
                            var prediction = cognitiveServicesResponse?.Classification?.Predictions?.FirstOrDefault();

                            if (prediction != null)
                            {
                                var res2 = botClient.SendMessage(message.Message.Chat.Id, $"Your TaxFree form issued by {prediction.TagName} has been acquired sucessfully!");
                                var res3 = botClient.SendMessage(message.Message.Chat.Id, $"Remember to validate the form at the Exit point to get your refund!");
                            }
                        }
                        else
                        {
                            var res4 = botClient.SendMessage(message.Message.Chat.Id, $"Hello, sorry I can't figure out what you sent me! Immagine: {resourceUrl}");
                        }

                    }
                    catch (System.Exception ex)
                    {
                        _logger.LogCritical(ex, ex.Message);
                    }
                }
            }
            return;
        }

        if (message.Message.Document != null)
        {
            BotClient botClient = new BotClient(_telegramOptions.Token);

            if (string.IsNullOrEmpty(message.Message.Caption))
            {
                var res = botClient.SendMessage(message.Message.Chat.Id, "Hello, sorry I can't figure out what you sent me!");
            }

            if (!string.IsNullOrEmpty(message.Message.Caption) && message.Message.Caption.ToLower().Contains("tff"))
            {
                var res = botClient.SendMessage(message.Message.Chat.Id, "Hello, thanks for send me your TaxFree Form. I'll process it and I'll let you know!");
            }
            return;
        }

        if (!CanCallGPT3(message.Message.Text))
        {
            return;
        }

        var response = await CallGPT3Async(new OpenAIRequest { prompt = message.Message.Text });

        if (response != null && response.Choices != null && response.Choices.Any())
        {
            var choice = response.Choices.FirstOrDefault();
            if (choice != null)
            {
                BotClient botClient = new BotClient(_telegramOptions.Token);

                var res = botClient.SendMessage(message.Message.From.Id, choice.text);
            }
        }
    }

    public async Task LogMessageAsync(string message)
    {
        GenocsChat chatMessage = new GenocsChat { Message = message };
        await _mongoDbRepository.InsertAsync(chatMessage);
    }
}