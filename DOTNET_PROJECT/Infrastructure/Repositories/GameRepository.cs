using DOTNET_PROJECT.Application.Interfaces.Repositories;
using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DOTNET_PROJECT.Infrastructure.Repositories;

public class GameRepository : GenericRepository<GameSave>, IGameRepository
{
    private readonly AppDbContext _dbContext;

    public GameRepository(AppDbContext context) : base(context) 
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<GameSave>> GetAllByUserId(int userId)
    {
        return await _dbContext.GameSaves
            .Where(gs => gs.UserId == userId)
            .OrderByDescending(gs => gs.LastUpdate)
            .ToListAsync();
    }
}
