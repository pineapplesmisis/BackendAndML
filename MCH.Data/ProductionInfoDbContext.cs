using MCH.Data.Configuration;
using MCH.Data.Entities;
using MCH.Parset.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MCH.Data
{
    public class ProductionInfoDbContext: IdentityDbContext<UserEntity, RoleEntity, int>
    {
        public  DbSet<CompanyEntity> CompanyEntities { get; set; }
        public  DbSet<ProductEntity> ProductEntities { get; set; }
        public  DbSet<UrlsToParseEntity> UrlsToParseEntities { get; set; }
        
        public  DbSet<ImageEntity> ImageEntities { get; set; }
        public ProductionInfoDbContext(DbContextOptions<ProductionInfoDbContext> options)
            : base(options)
        { }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new CompanyEntityConfiguration());
            builder.ApplyConfiguration(new UrlsToParseConfiguration());
            builder.ApplyConfiguration(new ProductEntityConfiguration());
            builder.ApplyConfiguration(new ImageEntityConfiguration());
        }
    }
}