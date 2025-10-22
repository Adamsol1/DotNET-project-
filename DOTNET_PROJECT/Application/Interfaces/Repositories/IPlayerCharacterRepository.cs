using DOTNET_PROJECT.Domain.Models;

namespace DOTNET_PROJECT.Application.Interfaces.Repositories;


public interface IPlayerCharacterRepository : IGenericRepository<PlayerCharacter>
{
    /// <summary>
    /// Get health of a player character by its ID.
    /// </summary>
    Task<int> GetHealthByIdAsync(int id);
    
    /// <summary>
    /// Get all player characters belonging to a specific user
    /// </summary>
    Task<IEnumerable<PlayerCharacter>> GetAllByUserId(int userId);
    
    /// <summary>
    /// Get player character by user ID (first one)
    /// </summary>
    Task<PlayerCharacter?> GetByUserId(int userId);
}