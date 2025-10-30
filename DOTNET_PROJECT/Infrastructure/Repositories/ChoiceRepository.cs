using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Application.Interfaces.Repositories;
using DOTNET_PROJECT.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace DOTNET_PROJECT.Infrastructure.Repositories;


public class ChoiceRepository : GenericRepository<Choice>, IChoiceRepository
{

    private readonly AppDbContext _db;
    public ChoiceRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    /// <summary>
    /// Get the StoryNode id this choice belongs to
    /// The method expects either one or zero results because the storynode ID is unique.
    /// </summary>

    public async Task<int> GetStoryNodeId(int id)
    {
        var storyNodeId = _db.Choices
                    .Where(choices => choices.Id == id)
                    .Select(choices => choices.StoryNodeId)
                    .SingleOrDefaultAsync();

        return await storyNodeId;
    }

    /// <summary>
    /// Get the StoryNode this choice belongs to
    /// The method expects either one or zero results because the storynode ID is unique.
    /// </summary>

    public async Task<StoryNode?> GetStoryNode(int id)
    {
        var storyNode = _db.Choices
                    .Where(choices => choices.Id == id)
                    .Select(choices => choices.StoryNode)
                    .SingleOrDefaultAsync();

        return await storyNode;
    }

    /// <summary>
    /// Get the next story node this choice leads to
    /// The method expects either one or zero results because the next storynode ID is unique.
    /// </summary>

    public async Task<StoryNode?> GetNextStoryNode(int id)
    {
        var nextStoryNodeId = _db.Choices
                    .Where(choices => choices.Id == id)
                    .Select(choices => choices.NextStoryNode)
                    .SingleOrDefaultAsync();

        return await nextStoryNodeId;
    }

    /// <summary>
    /// Get the id of the next story node this choice leads to
    /// The method expects either one or zero results because the storynode ID is unique.
    /// </summary>

    public async Task<int> GetNextStoryNodeId(int id)
    {
        var nextStoryNodeId = _db.Choices
                    .Where(choices => choices.Id == id)
                    .Select(choices => choices.NextStoryNodeId)
                    .SingleOrDefaultAsync();

        return await nextStoryNodeId;
    }

    /// <summary>
    /// Get the text given in this choice'
    /// </summary>
    public async Task<string> GetChoiceText(int id)
    {
        var choiceText = _db.Choices
                    .Where(choices => choices.Id == id)
                    .Select(choices => choices.Text)
                    .SingleOrDefaultAsync();

        return await choiceText;
    }

    public async Task<IEnumerable<Choice>> GetAllByStoryNodeId(int storyNodeId)
    {
        return await _db.Choices
            .Where(c => c.StoryNodeId == storyNodeId)
            .ToListAsync();
    }

}