using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCH.API.Models;
using MCH.Data.Entities;
using MCH.Parset.Data.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MCH.Core.Parsing
{
    public interface IParsingRepository
    {
        Task AddProductAsync(ProductEntity product);

        Task AddOrUpdateProductAsync(ProductEntity productEntity);
        Task AddCompanyAsync(CompanyEntity company);

        IEnumerable<CompanyEntity> getAllCompanies();

        IEnumerable<UrlsToParseEntity> getUrlsToParse(int companyId);

        CompanyEntity getCompanyEntity(int Id);

        Task AddImageIfNotExistAsync(ImageEntity image);
        

        ProductsListResponse GetProductsByCompany(int companyId, int count, int offset);

        ProductsListResponse GetProductsbyQuery(string query, int count, int offset);
        

        Task AddUrlToParseAsync(UrlsToParseEntity urlsToParse);

        int CountProducts(int companyId);

        ProductEntity GetProductById(int productId);

    }
}