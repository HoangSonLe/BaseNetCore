using Infrastructure.Services;

namespace BaseWebsite.BackgroundServices
{
    public class ReminderEmailBackgroundService : BackgroundService
    {
        private readonly ILogger<ReminderEmailBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        public ReminderEmailBackgroundService(ILogger<ReminderEmailBackgroundService> logger,
          IServiceScopeFactory serviceScopeFactory, IConfiguration configuration, EmailService emailService
          )
        {
            _logger = logger;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _emailService = emailService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRun = DateTime.Today.AddDays(1).AddHours(0).AddMinutes(0).AddSeconds(0);

                var delay = nextRun - now;
                if (delay.TotalMilliseconds <= 0)
                {
                    delay = TimeSpan.Zero;
                }
                _logger.LogInformation("ReminderEmailBackgroundService running at: {time}", DateTimeOffset.Now);

                //Wait until the next run time
                await Task.Delay(delay, stoppingToken);
                //Excute your daily task
                await SendReminderEmail(stoppingToken);
            }
        }
        private Task SendReminderEmail(CancellationToken stoppingToken)
        {
            // Your logic to run daily
            _logger.LogInformation("SendReminderEmail running at: {time}", DateTimeOffset.Now);
            var configNumberOfDaysNoticeAnniversary = int.Parse(_configuration.GetSection("NumberOfDaysNoticeAnniversary")?.Value);
            var reminderEmailSubject = _configuration.GetSection("ReminderEmailSubject")?.Value;
            //using (var serviceScope = _serviceScopeFactory.CreateScope())
            //{
            //    var _context = serviceScope.ServiceProvider.GetRequiredService<SampleReadOnlyDBContext>();
            //    var needSendEmailUrn = (from urn in _context.Urns.Where(i=> i.DeathDate.Date == DateTime.Now.AddDays(configNumberOfDaysNoticeAnniversary).Date)
            //                           join r in _context.Reminders on urn.Id equals r.UrnId
            //                           join u in _context.Users on r.UserId equals u.Id
            //                           select new {urn , reminder = r, user = u}).ToList();

            //    needSendEmailUrn.ForEach(u =>
            //    {
            //        if(Validate.ValidEmail(u.user.Email))
            //        {
            //            _ = _emailService.SendEmailAsync(u.user.Email, reminderEmailSubject, u.reminder.Content);
            //        }
            //    });
            //}

            // Simulate some work
            return Task.CompletedTask;
        }
    }
}
