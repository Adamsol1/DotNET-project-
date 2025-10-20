namespace DOTNET_PROJECT.Application.Dtos;
/// <summary>
/// Choice object that is sent between layers.
/// Contains all necessary data of a choice
/// </summary>
public sealed class ChoiceDto 
{
    /// <summary>
    /// Unique identifier of choice
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// The story node the choice is located on
    /// </summary>
    public int StoryNodeId { get; set; }
    /// <summary>
    /// The story node the choice will lead to
    /// </summary>
    public int NextStoryNodeId { get; set; }
    /// <summary>
    /// The text used to present the choice
    /// </summary>
    public string Text { get; set; } = string.Empty;
}

/// <summary>
/// Choice object used to send between layers when creating a new choice
/// </summary>
public sealed class CreateChoiceDto
{
    /// <summary>
    /// The unique identifier of the story node
    /// </summary>
    public int StoryNodeId { get; set; }
    /// <summary>
    /// The identifier of the next story node
    /// </summary>
    public int NextStoryNodeId { get; set; }
    /// <summary>
    /// The text used to present the choice
    /// </summary>
    public string Text { get; set; } = string.Empty;
}
/// <summary>
/// Choice object used to sendt data between different layers when updating a choice
/// </summary>
public sealed class UpdateChoiceDto
{
    /// <summary>
    /// Unique identifier of a choice
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// The identifer of the node the choice is used on
    /// </summary>
    public int StoryNodeId { get; set; }
    /// <summary>
    /// Tne identifier of the node the choice will lead to
    /// </summary>
    public int NextStoryNodeId { get; set; }
    /// <summary>
    /// The text used to present the choice
    /// </summary>
    public string Text { get; set; } = string.Empty;
}
