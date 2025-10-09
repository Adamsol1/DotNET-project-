using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
        /// Query to get user by their username
        var user = _db.User
                    .Where(User => User.Username == username)
                    .SingleOrDefaultAsync;

        return await user;
    }


    /// <summary>
    /// Get username associated with given user ID
    /// The method expects either one or zero results because usernames are unique.
    /// </summary>

    public async Task<string?> GetUsernameById(int id)
    {
        /// Query to get username by user ID
        var username = _db.User
                    .Where(User => User.Id == id)
                    .Select(User => User.Username)
                    .SingleOrDefaultAsync();

        return await username;
    }


    /// <summary>
    /// Limitied just for authentication testing purpose
    /// Get hashed password associated with given user ID
    /// </summary>

    public async Task<string?> GetPasswordHashById(int id)
    {
        /// Query to get hashed password by user ID
        var passwordHash = _db.User
                    .Where(User => User.Id == id)
                    .Select(User => User.PasswordHash)
                    .SingleOrDefaultAsync();

        return await passwordHash;
    }

    /// <summary>
    /// Get the role of the user associated with given user ID
    /// The method expects either one or zero results because only one role is given to each user
    /// </summary>
    
    public async Task<string?> GetUserRoleById(int id)
    {
        /// Query to get user role by user ID
        var userRole = _db.User
                    .Where(User => User.Id == id)
                    .Select(User => User.Role)
                    .SingleOrDefaultAsync();

        return await userRole;
    }


}