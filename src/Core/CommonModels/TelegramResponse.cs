using Core.Enums;

namespace Core.CommonModels
{

    public class TelegramResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TelegramChatId { get; set; }
        public ETelegramNotiType Type { get; set; }
        public string SentMessage { get; set; }
        public string ErrorMessage { get; set; } // This will be populated only in case of an error
    }

}
