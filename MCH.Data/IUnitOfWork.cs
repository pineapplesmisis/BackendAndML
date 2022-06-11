
using System.Threading.Tasks;
using MCH.Core.Parsing;

namespace CheckersBackend.Data
{
    public interface IUnitOfWork
    {
        IParsingRepository parsingRepository { get; set; }

        Task<int> CommitAsync();
    }
}
