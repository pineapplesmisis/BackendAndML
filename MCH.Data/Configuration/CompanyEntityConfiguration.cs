using MCH.Parset.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCH.Data.Configuration
{
    public class CompanyEntityConfiguration: IEntityTypeConfiguration<CompanyEntity>
    {
        public void Configure(EntityTypeBuilder<CompanyEntity> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .HasIndex(x => x.CompanyName)
                .IsUnique(true);

            builder
                .HasIndex(x => x.IIN)
                .IsUnique(true);

            builder
                .HasIndex(x => x.Url)
                .IsUnique(true);
        }
    }
}