using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCH.Data.Entities;
using MCH.Parset.Data.Entities;

namespace MCH.Core.Parsing
{
    public interface IParsingRepository
    {
        Task SaveProductAsync(ProductEntity product);
        Task SaveCompanyAsync(CompanyEntity company);

        IEnumerable<CompanyEntity> getAllCompanies();

        IEnumerable<UrlsToParseEntity> getUrlsToParse(int companyId);

        CompanyEntity getCompanyEntity(int Id);

        Task AddImageAsync(ImageEntity image);

        IEnumerable<CompanyEntity> GetCompanies();

        IEnumerable<ProductEntity> GetProductsByCompany(int companyId, int count);

        IEnumerable<ProductEntity> GetProductsbyQuery(string query, int count);

        Task CrateCompany(CompanyEntity companyEntity);

        Task CrateUrlToParseAsync(UrlsToParseEntity urlsToParse);

        CompanyEntity GetCompanyById(int Id);
    }
}