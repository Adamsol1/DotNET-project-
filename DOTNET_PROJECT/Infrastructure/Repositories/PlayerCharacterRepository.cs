using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Infrastructure.Data;
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
        var health = _db.PlayerCharacter
                    .Where(PlayerCharacter => PlayerCharacter.Id == id)
                    .Select(PlayerCharacter => PlayerCharacter.Health)
                    .SingleOrDefaultAsync();

        return await health;
    }
}