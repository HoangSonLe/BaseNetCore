using Infrastructure.Services;

namespace BaseWebsite.BackgroundServices
{
    public class BotHostedBackgroundService : BackgroundService
    {
        private readonly TelegramService _telegramService;

        public BotHostedBackgroundService(TelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _telegramService.Start();
            await Task.CompletedTask; // Keep the service running
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            // Perform cleanup if necessary
            return base.StopAsync(stoppingToken);
        }
    }
}
