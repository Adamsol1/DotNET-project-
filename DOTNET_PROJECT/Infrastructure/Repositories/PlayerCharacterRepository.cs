using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace DOTNET_PROJECT.Infrastructure.Repositories;


public class PlayerCharacterRepository : GenericRepository<PlayerCharacter>, IPlayerCharacterRepository
{
    private readonly AppDbContext _db;
    public PlayerCharacterRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    /// <summary>
    /// Get health of a player character by its ID.
    /// </summary>
    
    public async Task<int> GetHealthByIdAsync(int id)
    {
        /// Query to get health of a player character by its ID.
        var health = _db.Characters.OfType<PlayerCharacter>()
                    .Where(PlayerCharacter => PlayerCharacter.Id == id)
                    .Select(PlayerCharacter => PlayerCharacter.Health)
                    .SingleOrDefaultAsync();

        return await health;
    }

    public async Task<IEnumerable<PlayerCharacter>> GetAllByUserId(int userId)
    {
        return await GetAllByProperty(pc => pc.UserId, userId);
    }

    public async Task<PlayerCharacter?> GetByUserId(int userId)
    {
        return await GetByProperty(pc => pc.UserId, userId);
    }
}