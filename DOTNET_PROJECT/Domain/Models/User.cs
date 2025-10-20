namespace DOTNET_PROJECT.Domain.Models;
/// <summary>
/// Represents a user in the system.
/// </summary>
public class User
{
    /// <summary>
    /// The unique identifier for the user.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The username of the user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The hashed password of the user.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The role of the user (e.g., Admin, Player).
    /// </summary>
    public UserRole Role { get; set; } = UserRole.player;
    
    //TODO: Uncomment this when GameSave (save progresstions) are to be implemented
    
    // public ICollection<GameSave> GameSaves { get; set; } = new List<GameSave>();
    
}

/// <summary>
///  Will be implemented in future
/// </summary>
public enum UserRole{
    admin,
    player
}