using DOTNET_PROJECT.Domain.Models;

namespace DOTNET_PROJECT.Application.Interfaces;

public interface IGameRepository : IGenericRepository<GameSave>
{
    Task<IEnumerable<GameSave>> GetAllByUserId(int userId);
}
