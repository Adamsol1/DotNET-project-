
namespace WebProject_3200.Infrastructure.Models;
/// <summary>
/// Represents a player character in the story, inheriting from Character.
/// </summary>
public class PlayerCharacter : Character
{
    /// <summary>
    /// The health points of the player character.
    /// </summary>
    public int Health { get; set; } = 100;

}