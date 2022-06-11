using MCH.Data.Entities;
using MCH.Parset.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCH.Data.Configuration
{
    public class UrlsToParseConfiguration: IEntityTypeConfiguration<UrlsToParseEntity>
    {
        public void Configure(EntityTypeBuilder<UrlsToParseEntity> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .HasIndex(x => x.Url)
                .IsUnique(true);

            builder
                .HasOne<CompanyEntity>(x => x.Company)
                .WithMany(x => x.urls)
                .HasForeignKey(x => x.CompanyId);
        }
    }
}