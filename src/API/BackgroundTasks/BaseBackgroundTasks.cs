using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.BackgroundTasks
{
    public abstract class BaseBackgroundTasks<T> : BackgroundService
    {
        protected readonly ILogger<T> _logger;
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly IConfiguration _configuration;
        public BaseBackgroundTasks(ILogger<T> logger,
            IServiceScopeFactory serviceScopeFactory, IConfiguration configuration
            )
        {
            _logger = logger;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Calculate the next desired run time
            DateTimeOffset nextRunTime = CalculateNextRunTime();

            while (!stoppingToken.IsCancellationRequested)
            {
                DateTimeOffset currentTime = DateTimeOffset.Now;

                if (currentTime >= nextRunTime)
                {
                    _logger.LogInformation("Background service is running at: {time}", currentTime);

                    // Perform your task here
                    BackgroundTask(currentTime);
                    // Calculate the next desired run time
                    nextRunTime = CalculateNextRunTime();
                }

                // Wait for a short interval before checking again
                await Task.Delay(TimeSpan.FromSeconds(50), stoppingToken);
            }
            _logger.LogInformation("Background service is stopping.");
        }
        public virtual DateTimeOffset CalculateNextRunTime()
        {
            // Calculate and return the next desired run time based on your schedule logic
            // For example, you can set it to run every day at a specific time
            // Here's an example of running at 00:01 AM every day
            DateTimeOffset now = DateTimeOffset.Now;
            DateTimeOffset nextRunTime = now.Date.AddHours(24).AddMinutes(10); // 00:10 AM

            if (now >= nextRunTime)
            {
                nextRunTime = nextRunTime.AddDays(1); // Move to the next day
            }

            return nextRunTime;
        }
        public abstract void BackgroundTask(DateTimeOffset currentTime);
    }
}
