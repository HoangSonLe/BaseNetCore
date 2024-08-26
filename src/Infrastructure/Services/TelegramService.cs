using AutoMapper;
using Core.CommonModels;
using Core.CommonModels.BaseModels;
using Core.CoreUtils;
using Core.Enums;
using Infrastructure.DBContexts;
using Infrastructure.Entitites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Services
{
    public class TelegramService
    {
        private readonly string _token;
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly TelegramBotClient _botClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TelegramService> _logger;
        private readonly HttpClient _httpClient;

        public TelegramService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<TelegramService> logger,
            IServiceScopeFactory serviceScopeFactory,
            IMapper mapper
        )
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;
            _token = _configuration.GetSection("TelegramToken").Value;
            _botClient = new TelegramBotClient(_token);
        }

        public void Start()
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions
            );
        }
        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message)
            {
                if (update.Message.Text == "/start")
                {
                    await RequestPhoneNumber(botClient, update.Message.Chat.Id, cancellationToken);
                }
                else if (update.Message.Contact != null)
                {
                    var ack = await SaveTelegramChat(update.Message.Chat.Id.ToString(), update.Message.Contact.PhoneNumber);
                    var messeage = $"Cảm ơn bạn đã đăng ký nhận tin! Số điện thoại của bạn là {update.Message.Contact.PhoneNumber}";
                    if (ack.IsSuccess == false)
                    {
                        messeage = ack.ErrorMessageList.FirstOrDefault();
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: messeage,
                        cancellationToken: cancellationToken
                    );
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: "Hệ thống không hỗ trợ nhắn tin trực tiếp.Vui lòng liên hệ với người quản lý.",
                        cancellationToken: cancellationToken
                    );
                }
            }
        }
        private async Task<Acknowledgement> SaveTelegramChat(string telegramChatId, string phoneNumber)
        {
            var response = new Acknowledgement();
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                response.AddMessage("Số điện thoại không hợp lệ");
                return response;
            }
            var isValidPhone = Validate.ValidPhoneNumber(ref phoneNumber);
            if (isValidPhone != null)
            {
                response.AddMessage(isValidPhone.ToString());
                return response;
            }
            using (var serviceScope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    var _context = serviceScope.ServiceProvider.GetRequiredService<SampleDBContext>();
                    var user = await _context.Users.FirstOrDefaultAsync(i => i.Phone == phoneNumber && i.State == (int)EState.Active);
                    if (user == null)
                    {
                        response.AddMessage($"Người dùng với số điện thoại {phoneNumber} chưa được đăng ký trong phần mềm quản lý Linh Cốt.Vui lòng đăng ký tài khoản người dùng với số điện thoại này.");
                        return response;
                    }
                    var existChat = await _context.TelegramChats.FirstOrDefaultAsync(i => i.UserId == user.Id);
                    if (existChat != null)
                    {
                        existChat.TelegramChatId = telegramChatId;
                        _context.TelegramChats.Update(existChat);
                    }
                    else
                    {
                        var newChat = new TelegramChat()
                        {
                            UserId = user.Id,
                            PhoneNumber = phoneNumber,
                            TelegramChatId = telegramChatId,
                        };
                        await _context.TelegramChats.AddAsync(newChat);
                    }

                    await _context.SaveChangesAsync();
                    response.IsSuccess = true;
                    return response;
                }
                catch (Exception ex)
                {
                    response.AddMessage($"Error: {ex.Message}");
                    return response;
                }

            }
        }
        private async Task RequestPhoneNumber(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            var requestPhoneNumberKeyboard = new ReplyKeyboardMarkup(new[]
            {
            new KeyboardButton("Share Contact") { RequestContact = true }
        })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Vui lòng chia sẻ số điện thoại để nhận thông báo bằng cách nhấn nút Share Contact bên dưới.",
                replyMarkup: requestPhoneNumberKeyboard,
                cancellationToken: cancellationToken
            );
        }
        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        public async Task<HttpResponseMessage> SendMessageAsync(string telegramChatId, string message, ETelegramNotiType type)
        {
            const int maxRetryAttempts = 3;
            int retryCount = 0;
            HttpResponseMessage response = null;
            bool successful = false;

            while (retryCount < maxRetryAttempts && !successful)
            {
                try
                {
                    var url = $"https://api.telegram.org/bot{_token}/sendMessage";
                    var content = new StringContent(JsonSerializer.Serialize(new
                    {
                        chat_id = telegramChatId,
                        text = message,
                        parse_mode = "HTML"
                    }), System.Text.Encoding.UTF8, "application/json");

                    response = await _httpClient.PostAsync(url, content);

                    response.EnsureSuccessStatusCode(); // Throws an exception if the status code is not successful
                    successful = true; // If no exception is thrown, consider the operation successful
                }
                catch (HttpRequestException ex)
                {
                    retryCount++;

                    if (retryCount >= maxRetryAttempts)
                    {
                        _logger.LogError($"Failed to send message after {maxRetryAttempts} attempts: {ex.Message}", ex);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(2)); // Optional: Delay before retrying
                }
            }
            if (successful)
            {
                var successContent = new TelegramResponse
                {
                    Success = true,
                    Message = "Message sent successfully",
                    TelegramChatId = telegramChatId,
                    SentMessage = message,
                    Type = type
                };
                response.Content = new StringContent(JsonSerializer.Serialize(successContent), System.Text.Encoding.UTF8, "application/json");
            }

            return response;
        }
    }

}

