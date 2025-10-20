namespace DOTNET_PROJECT.Application.Dtos;
/// <summary>
/// Represents a story node in the game.
/// An object sent between different layers containing all necessary data. 
/// </summary>
public sealed class StoryNodeDto
{
    /// <summary>
    /// Unique identifier of a story node
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// The title of the storynode
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// The description of the story node
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// The url of the background image used in the storynode
    /// </summary>
    public string BackgroundUrl { get; set; } = string.Empty;
    /// <summary>
    /// A list of all dialogues used in the storynode
    /// </summary>
    public List<DialogueDto> Dialogues { get; set; } = new List<DialogueDto>();
    /// <summary>
    /// A list of all choices given in the story node
    /// </summary>
    public List<ChoiceDto> Choices { get; set; } = new List<ChoiceDto>();
}


// <summary>
/// Represents a story node in the game.
/// A object sent between different layers containing all necessary datawhn creating a new story node.
/// Note : Id is given automatically in the database. 
/// </summary>
public sealed class CreateStoryNodeDto {
    /// <summary>
    /// The title of a story node
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// The description of a story node
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// The background url of the image used in the storynode
    /// </summary>
    public string BackgroundUrl { get; set; } = string.Empty;
}
// <summary>
/// Represents a story node in the game.
/// A object sent between different layers containing all necessary data when updating a story node
/// </summary>
public sealed class UpdateStoryNodeDto {
    /// <summary>
    /// Unique identifier of a story node
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// The title of the story node
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// The description of the story node
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// The url of the image used as background
    /// </summary>
    public string BackgroundUrl { get; set; } = string.Empty;
}