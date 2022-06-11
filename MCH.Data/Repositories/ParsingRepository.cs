using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MCH.Data;
using MCH.Data.Entities;
using MCH.Parset.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MCH.Core.Parsing
{
    public class ParsingRepository: IParsingRepository
    {
        private readonly ProductionInfoDbContext _context;
        public ParsingRepository(ProductionInfoDbContext context)
        {
            _context = context;
        }
        public async Task SaveProductAsync(ProductEntity product)
        {
            await _context.ProductEntities.AddAsync(product);
        }

        public async Task SaveCompanyAsync(CompanyEntity company)
        {
            await _context.CompanyEntities.AddAsync(company);
        }

        public IEnumerable<CompanyEntity> getAllCompanies()
        {
            return _context.CompanyEntities;
        }

        public IEnumerable<UrlsToParseEntity> getUrlsToParse(int companyId)
        {
            return _context.UrlsToParseEntities
                .Where(u => u.CompanyId == companyId);
        }

        public CompanyEntity getCompanyEntity(int Id)
        {
            return _context.CompanyEntities.First(c => c.Id == Id);
        }

        public async Task AddImageAsync(ImageEntity image)
        {
            await _context.ImageEntities.AddAsync(image);
        }

        public IEnumerable<CompanyEntity> GetCompanies()
        {
            return _context.CompanyEntities;
        }

        public IEnumerable<ProductEntity> GetProductsByCompany(int companyId, int count)
        {
            return _context.ProductEntities
                .Include(p => p.Images)
                .Where(p => p.CompanyId == companyId).Take(count);
        }

        public IEnumerable<ProductEntity> GetProductsbyQuery(string query, int count)
        {
            return _context.ProductEntities
                .Include(p => p.Images)
                .Take(count);
        }

        public async Task CrateCompany(CompanyEntity companyEntity)
        {
            await _context.CompanyEntities.AddAsync(companyEntity);
        }

        public async Task CrateUrlToParseAsync(UrlsToParseEntity urlsToParse)
        {
            await _context.UrlsToParseEntities.AddAsync(urlsToParse);
        }

        public CompanyEntity GetCompanyById(int Id)
        {
            return _context.CompanyEntities.FirstOrDefault(c => c.Id == Id);
        }
    }
}