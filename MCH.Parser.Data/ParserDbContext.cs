using Microsoft.EntityFrameworkCore;

namespace MCH.Parset.Data
{
    public class ParserDbContext: DbContext
    {
        public PetitionAppDbContext(DbContextOptions<PetitionAppDbContext> options)
            : base(options)
        { }
    }
}