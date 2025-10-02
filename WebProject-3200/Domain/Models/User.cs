
namespace WebProject_3200.Domain.Models;
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
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// The role of the user (e.g., Admin, Player).
    /// </summary>
    public string Role { get; set; } = "Player";
}