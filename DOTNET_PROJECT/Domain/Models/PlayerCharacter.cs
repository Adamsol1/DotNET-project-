namespace DOTNET_PROJECT.Domain.Models;
/// <summary>
/// Represents a player character in the story, inheriting from Character.
/// </summary>
public class PlayerCharacter : Character
{

    public int Health { get; set; } = 100;
    
    //TODO: Delete these below and change follow consequences to use GameSave instead
    //These shall be removed as deprecated --> We use GameSave now
    public int UserId { get; set; }
    public int CurrentStoryNodeId { get; set; }
    public User User { get; set; } = null!;
    public StoryNode CurrentStoryNode { get; set; } = null!;
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
}