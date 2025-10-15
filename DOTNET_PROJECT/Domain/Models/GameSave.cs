namespace DOTNET_PROJECT.Domain.Models;

public class GameSave
{
    public int Id { get; set; }
    
    //User connection
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    //PlayerCharacter connection
    public int PlayerCharacterId { get; set; }
    public PlayerCharacter? PlayerCharacter { get; set; }
    
    //Which story-node the player is at
    public int CurrentStoryNodeId { get; set; }
    public StoryNode? CurrentStoryNode { get; set; }
    
    //Save info
    public String SaveName { get; set; }
    public DateTime LastUpdate { get; set; }
}