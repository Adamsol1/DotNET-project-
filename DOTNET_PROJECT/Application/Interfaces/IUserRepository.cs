using DOTNET_PROJECT.Domain.Models;

namespace DOTNET_PROJECT.Application.Interfaces;

public interface UserRepository : IGenericRepository<User>
{
    /// <summary>
    /// Get user by their username
    /// </summary>
    Task<User?> GetUserByUsername(string username);

    /// <summary>
    /// Get username associated with given user ID
    /// </summary>
    Task<string?> GetUsernameById(int id);


    /// <summary>
    /// Limitied just for authentication testing purpose
    /// Get hashed password associated with given user ID
    /// </summary>
    Task<string?> GetPasswordHashById(int id);

    /// <summary>
    /// Get the role of the user associated with given user ID
    /// </summary>
    
    Task<string> GetUserRoleById(int id);
}