

namespace DOTNET_PROJECT.Domain.Models;

/// <summary>
/// Represents a choice in a story node that leads to another story node.
/// </summary>
public class Choice
{
    /// <summary>
    /// The unique identifier for the choice.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The identifier of the story node this choice belongs to.
    /// </summary>
    public int StoryNodeId { get; set; }

    /// <summary>
    /// The story node this choice belongs to.
    /// </summary>
    public StoryNode StoryNode { get; set; } = null!;
    
    /// <summary>
    /// The identifier of the next story node this choice leads to.
    /// </summary>
    public int NextStoryNodeId { get; set; }
    public StoryNode? NextStoryNode { get; set; }

    /// <summary>
    /// The text displayed for this choice.
    /// </summary>
    public string Text { get; set; } = string.Empty;
}