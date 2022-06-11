using System.Threading.Tasks;
using CheckersBackend.Data;
using MCH.Core.Parsing;

namespace MCH.Data
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ProductionInfoDbContext _context;
        public IParsingRepository parsingRepository { get; set; }

        public UnitOfWork(IParsingRepository parsingRepository, ProductionInfoDbContext context)
        {
            this.parsingRepository = parsingRepository;
            _context = context;
        }
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}