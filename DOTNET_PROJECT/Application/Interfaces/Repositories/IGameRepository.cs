using DOTNET_PROJECT.Domain.Models;

namespace DOTNET_PROJECT.Application.Interfaces.Repositories;

public interface IGameRepository : IGenericRepository<GameSave>
{
    Task<IEnumerable<GameSave>> GetAllByUserId(int userId);
}
