using DOTNET_PROJECT.Domain.Models;

namespace DOTNET_PROJECT.Application.Interfaces;


public interface DialogueRepository : IGenericRepository<Dialogue>
{
    /// <summary>
    /// Get the id of the story node where dialogue is shown
    /// </summary>
    Task<int> GetStoryNodeId(int dialogueId);

    /// <summary>
    /// Get the storynode the dialogue is shown in
    /// </summary>
    Task<StoryNode> GetStoryNode(int dialogueId);

    /// <summary>
    /// Get the order the dialogues in current story node is shown
    /// </summary>
    Task<int> GetDialogueOrder(int dialogueId);

    /// <summary>
    /// Get the character id of the character speaking the dialogue
    /// </summary>
    Task<int> GetCharacterId(int dialogueId);

    /// <summary>
    /// Optional as not all dialogues have a character. For example, narration and sound effects. 
    /// Get the character speaking the dialogue
    /// </summary>
    Task<Character> GetCharacter(int dialogueId);

    /// <summary>
    /// Get the dialogues text
    /// </summary>
    Task<string> GetDialogueText(int dialogueId);
}