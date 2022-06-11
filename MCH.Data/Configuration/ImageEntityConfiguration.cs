using MCH.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCH.Data.Configuration
{
    public class ImageEntityConfiguration: IEntityTypeConfiguration<ImageEntity>
    {
        public void Configure(EntityTypeBuilder<ImageEntity> builder)
        {
            builder
                .HasKey(x => x.Id);
            
            builder
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder
                .HasOne<ProductEntity>(x => x.ProductEntity)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.ProductId);

        }
    }
}