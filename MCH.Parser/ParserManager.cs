using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CheckersBackend.Data;
using MCH.Configuration;
using MCH.Parsers;
using MCH.Parset.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MCH
{
    public class ParserManager
    {
        private   IServiceScopeFactory _serviceScopeFactory;
        private AppSettings _appSettings;
        private readonly ILogger _logger;

        public ParserManager(IOptions<AppSettings> settings, ILogger logger)
        {
            _logger = logger;
            _appSettings = settings.Value;
        }
        public async Task Start(IServiceScopeFactory serviceScopeFactory,  CancellationToken token)
        {
            _serviceScopeFactory = serviceScopeFactory;
            
            while (!token.IsCancellationRequested)
            {
                
                
                foreach (var company in getCompanies())
                {
                    MainParser parser = new(serviceScopeFactory, _appSettings.XmlFilesFolder, _logger);
                     Task.Run(async () => await parser.Start(company.Id));
                     _logger.LogInformation($"Start parsing site of company: {company.CompanyName}");
                }

                await Task.Delay(10000000, token);
            }
        }

        private List<CompanyEntity> getCompanies()
        { 
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                return unitOfWork.parsingRepository.getAllCompanies()
                    .Skip(10).ToList();

            }
        }
    }
}