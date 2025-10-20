namespace DOTNET_PROJECT.Application.Dtos;

/// <summary>
/// Dialogue object used to send data between different layers.
/// Includes all necessary data for a dialogue
/// </summary>
public sealed class DialogueDto
{
    /// <summary>
    /// Unique identifier of a dialogue
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// The story node where the dialogue is found
    /// </summary>
    public int StoryNodeId { get; set; }
    /// <summary>
    /// Represents when the dialogue will be displayed. 
    /// </summary>
    public int Order { get; set; }
    /// <summary>
    /// The identifier of the character this dialogue belongs to
    /// </summary>
    public int CharacterId { get; set; }
    /// <summary>
    /// The text used to present the dialogue
    /// </summary>
    public int? CharacterId { get; set; }
    public string Text { get; set; } = string.Empty;
}
/// <summary>
/// Dialogue object used to send data between different layers when creating a new dialogue
/// Includes all necessary data for a dialogue
/// </summary>
public sealed class CreateDialogueDto
{
    /// <summary>
    /// The unique identifier of a story node
    /// </summary>
    public int StoryNodeId { get; set; }
    /// <summary>
    /// The order the dialogue will be displayed
    /// </summary>
    public int Order { get; set; }
    /// <summary>
    /// The identifier of the character this dialogue belongs to
    /// </summary>
    public int CharacterId { get; set; }
    /// <summary>
    /// The text used to present the dialogue
    /// </summary>
    public string Text { get; set; } = string.Empty;
}
/// <summary>
/// Dialogue object used to send data between different layers when updating the dlalogue
/// Includes all necessary data for a dialogue
/// </summary>
public sealed class UpdateDialogueDto
{
    /// <summary>
    /// The unique identifer of the dialogue
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// The id of the story node this dialogue is connected to
    /// </summary>
    public int StoryNodeId { get; set; }
    /// <summary>
    /// The given order the dialogue is being displayed
    /// </summary>
    public int Order { get; set; }
    /// <summary>
    /// The id of the character this dialogue belongs to
    /// </summary>
    public int CharacterId { get; set; }
    /// <summary>
    /// The text used to display the dialogue
    /// </summary>
    public string Text { get; set; } = string.Empty;
}

