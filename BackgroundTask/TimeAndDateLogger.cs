namespace ScheduledFileLogger.BackgroundTask
{
    public class TimeAndDateLogger
    {
        public class LogFileGenerationService : IHostedService

        {
            private readonly ILogger<LogFileGenerationService> _logger;
            private readonly string _filePath;

            public LogFileGenerationService(ILogger<LogFileGenerationService> logger, IConfiguration configuration)
            {
                _logger = logger;


                var projectPath = Directory.GetCurrentDirectory();


                _filePath = Path.Combine(projectPath, "ScheduleLog", "Log.txt");


                if (!Directory.Exists(Path.GetDirectoryName(_filePath)))
                {
                    _ = Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
                }
            }

            public async Task StartAsync(CancellationToken stoppingToken)
            {
                await WriteLogAsync(stoppingToken);
            }

            public async Task StopAsync(CancellationToken stoppingToken)
            {
                await WriteLogAsync(stoppingToken);
            }

            private async Task WriteLogAsync(CancellationToken stoppingToken)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var currentDateTime = DateTimeOffset.UtcNow;
                        var time = currentDateTime.ToLocalTime();
                        var timestamp = $"Time: {time.TimeOfDay} ---- Date: {currentDateTime:dd/MM/yyyy}";

                        var content = $"Log entry for date and time at: {timestamp}";

                        await WriteToFileAsync(content);
                        _logger.LogInformation($"File created at: {_filePath}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating file");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
                }
            }

            private async Task WriteToFileAsync(string content)
            {
                using var streamWriter = new StreamWriter(_filePath, true);
                await streamWriter.WriteLineAsync(content);
            }
        }
    }

}
