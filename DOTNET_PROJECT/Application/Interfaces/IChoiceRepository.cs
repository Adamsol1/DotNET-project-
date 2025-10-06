using DOTNET_PROJECT.Domain.Models;

namespace DOTNET_PROJECT.Application.Interfaces;


public interface ChoiceRepository : IGenericRepository<Choice>
{
    /// <summary>
    /// Get StoryNode id this choice belongs to
    /// </summary>
    Task<int> GetStoryNodeId(int choiceId);

    /// <summary>
    /// Get StoryNode this choice belongs to
    /// </summary>

    Task<StoryNode> GetStoryNode(int choiceId);

    /// <summary>
    /// Get id of story node this choice leads to
    /// </summary>

    Task<StoryNode> GetNextStoryNodeId(int choiceId);

    /// <summary>
    /// Get the story node this choice leads to'
    /// </summary>
    Task<StoryNode> GetNextStoryNode(int choiceId);
    
    /// <summary>
    /// Get text given in this choice
    /// </summary>
    Task<string> GetChoiceText(int choiceId);
}