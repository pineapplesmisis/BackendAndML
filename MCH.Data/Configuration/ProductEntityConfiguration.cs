using MCH.Data.Entities;
using MCH.Parset.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCH.Data.Configuration
{
    public class ProductEntityConfiguration: IEntityTypeConfiguration<ProductEntity>
    {
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            builder
                .HasKey(x => x.Id);
            
            builder
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder
                .HasOne<CompanyEntity>(x => x.company)
                .WithMany(x => x.products)
                .HasForeignKey(x => x.CompanyId);
        }
    }
}