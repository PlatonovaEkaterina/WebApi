using WorkerService.Settings;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly HttpClient _httpClient;
        private readonly ShopSettings _settings;

        public Worker(ILogger<Worker> logger, HttpClient httpClient, ShopSettings shopSettings)
        {
            _logger = logger;
            _httpClient = httpClient;
            _settings = shopSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                var response = await _httpClient.GetStringAsync($"{_settings.Url}/api/v1/users");

                var f = response;
                
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
