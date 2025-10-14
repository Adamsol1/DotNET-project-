using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace DOTNET_PROJECT.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    /// <summary>
    /// Get user by their username
    /// The method expects either one or zero results because usernames are unique.
    /// </summary>

    public async Task<User?> GetUserByUsername(string username)
    {
        return await _db.User.FirstOrDefaultAsync(u => u.Username == username);
    }


    /// <summary>
    /// Get username associated with given user ID
    /// The method expects either one or zero results because usernames are unique.
    /// </summary>

    public async Task<string?> GetUsernameById(int id)
    {
        /// Query to get username by user ID
        return await _db.User.FirstOrDefaultAsync(u => u.Id == id)?.Username;
    }


    /// <summary>
    /// Limitied just for authentication testing purpose
    /// Get hashed password associated with given user ID
    /// </summary>

    public async Task<string?> GetPasswordById(int id)
    {
        /// Query to get hashed password by user Id
        return await _db.User.FirstOrDefaultAsync(u => u.Id == id)?.Password;
    }

    /// <summary>
    /// Get the role of the user associated with given user ID
    /// The method expects either one or zero results because only one role is given to each user
    /// </summary>
    
    public async Task<string?> GetUserRoleById(int id)
    {
        /// Query to get user role by user Id
        return await _db.User.FirstOrDefaultAsync(u => u.Id == id)?.Role;
    }


}