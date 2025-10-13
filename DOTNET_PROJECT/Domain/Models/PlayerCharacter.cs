namespace DOTNET_PROJECT.Domain.Models;
/// <summary>
/// Represents a player character in the story, inheriting from Character.
/// </summary>
public class PlayerCharacter
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int CurrentStoryNodeId { get; set; }
    public int Health { get; set; } = 100;

    public User User { get; set; } = null!;
    public StoryNode CurrentStoryNode { get; set; } = null!;

    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
}