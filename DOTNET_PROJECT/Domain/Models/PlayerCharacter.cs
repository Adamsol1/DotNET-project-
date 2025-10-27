namespace DOTNET_PROJECT.Domain.Models;
/// <summary>
/// Represents a player character in the story, inheriting from Character.
/// </summary>
public class PlayerCharacter : Character
{
    public int Health { get; set; } = 100;
    public int UserId { get; set; }
    public int CurrentStoryNodeId { get; set; }
}