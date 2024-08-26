using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace API.BackgroundTasks
{
    public class ExampleBackground : BaseBackgroundTasks<ExampleBackground>
    {
        public ExampleBackground(ILogger<ExampleBackground> logger,
            IServiceScopeFactory serviceScopeFactory, IConfiguration configuration
            ) : base(logger, serviceScopeFactory, configuration)
        {

        }
        public override DateTimeOffset CalculateNextRunTime()
        {
            return base.CalculateNextRunTime();
        }
        public override void BackgroundTask(DateTimeOffset currentTime)
        {
            _logger.LogInformation("Running.....");
        }
    }
}
