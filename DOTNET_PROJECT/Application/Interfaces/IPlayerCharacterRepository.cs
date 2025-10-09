using DOTNET_PROJECT.Domain.Models;

namespace DOTNET_PROJECT.Application.Interfaces;


public interface IPlayerCharacterRepository : IGenericRepository<PlayerCharacter>
{
    /// <summary>
    /// Get health of a player character by its ID.
    /// </summary>

    Task<int> GetHealthByIdAsync(int id);
}