namespace WebProject_3200.Infrastructure.Models;
/// <summary>
/// Represents a node in the story, containing dialogues and choices.
/// </summary>
public class StoryNode
{
    /// <summary>
    /// The unique identifier for the story node.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The title of the story node.
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// A brief description of the story node.
    /// </summary>
    public string Desciption { get; set; } = string.Empty;
    /// <summary>
    /// URL to a background image for the story node.
    /// </summary>
    public string? BackgroundUrl { get; set; } = string.Empty;

    /// <summary>
    /// Dialogues associated with the story node.
    /// </summary>
    public ICollection<Dialogue> Dialogues { get; set; } = new List<Dialogue>();

    /// <summary>
    /// Choices available at the story node.
    /// </summary>
    public ICollection<Choice> Choices { get; set; } = new List<Choice>();
}
    
