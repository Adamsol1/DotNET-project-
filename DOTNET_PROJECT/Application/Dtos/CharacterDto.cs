namespace DOTNET_PROJECT.Application.Dtos;

    /// <summary>
    /// Character object that is used to send data between layers.
    /// Contains necessary data to define a character in the game
    /// </summary>
public sealed class CharacterDto
{
    /// <summary>
    /// Unique identifier of character
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Name of character
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Description of the character
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// The url of the image of the given character
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;
    /// <summary>
    /// A list of all the dialogues of this character
    /// </summary>
    public List<DialogueDto> Dialogues { get; set; } = new List<DialogueDto>();
}
/// <summary>
/// A character object that is sent between layers when creating a new character
/// </summary>
public sealed class CreateCharacterDto
{
    /// <summary>
    /// The name of the character that is being created
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The description of the character that is being created
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// The url of the image of the given character
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;
}

/// <summary>
/// Character object that is sent between layers when updating a character
/// </summary>
public sealed class UpdateCharacterDto
{
    /// <summary>
    /// Unique identifier of character
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Name of the character
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Description of the character
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// The url of the image connected to the character
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;
}
