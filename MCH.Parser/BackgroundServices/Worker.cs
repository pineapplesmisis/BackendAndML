using System.Threading;
using System.Threading.Tasks;
using CheckersBackend.Data;
using MCH.Configuration;
using MCH.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MCH.BackgroundServices
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ParserManager _manager;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Worker(ILogger<Worker> logger, IOptions<AppSettings> settings, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _manager = new(settings, _logger);
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           await _manager.Start(_serviceScopeFactory, stoppingToken);
        }
    }
}