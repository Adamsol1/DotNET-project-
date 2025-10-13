namespace DOTNET_PROJECT.Domain.Models;
/// <summary>
/// Represents a player character in the story, inheriting from Character.
/// </summary>
public class PlayerCharacter : Character
{
    
    public User userId {get; set;}
    public StoryNode CurrentStoryNode {get; set;}
    /// <summary>
    /// The health points of the player character.
    /// </summary>
    public int Health { get; set; } = 100;

}