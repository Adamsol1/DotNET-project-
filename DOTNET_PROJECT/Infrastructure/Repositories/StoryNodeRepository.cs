using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Application.Interfaces.Repositories;
using DOTNET_PROJECT.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DOTNET_PROJECT.Infrastructure.Repositories;

public class StoryNodeRepository : GenericRepository<StoryNode>, IStoryNodeRepository
{
    private readonly AppDbContext _db;
    public StoryNodeRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    /// <summary>
    /// Get the story node title with ID
    /// The method expects either one or zero results because the storynode ID is unique.
    /// </summary>

    public async Task<String> GetStoryNodeTitleById(int id)
    {
        var title = _db.StoryNodes
                    .Where(storyNodes => storyNodes.Id == id)
                    .Select(storyNodes => storyNodes.Title)
                    .SingleOrDefaultAsync();

        return await title;
    }

    /// <summary>
    /// Get the story node with given title
    /// </summary>

    public async Task<StoryNode?> GetStoryNodeByTitle(string title)
    {
        var storyNode = _db.StoryNodes
                    .Where(storyNodes => storyNodes.Title == title)
                    .FirstOrDefaultAsync();

        return await storyNode;
    }

    /// <summary>
    /// Get description of the story node with given ID
    /// </summary>
    public async Task<String> GetStoryNodeDescription(int id)
    {
        var description = _db.StoryNodes
                    .Where(storyNodes => storyNodes.Id == id)
                    .Select(storyNodes => storyNodes.Description)
                    .SingleOrDefaultAsync();

        return await description;
    }

    /// <summary>
    /// Get the URL of the StoryNode background given by ID
    /// </summary>

    public async Task<String> GetStoryNodeBackgroundUrl(int id)
    {
        var background = _db.StoryNodes
                    .Where(storyNodes => storyNodes.Id == id)
                    .Select(storyNodes => storyNodes.BackgroundUrl)
                    .SingleOrDefaultAsync();

        return await background;
    }

    /// <summary>
    /// Get all dialougues associated with a story node given by ID
    /// </summary>

    public async Task<IEnumerable<Dialogue>> GetAllDialoguesOfStoryNode(int id)
    {
        var dialogues = _db.Dialogues
                    .Where(dialogues => dialogues.StoryNodeId == id)
                    .ToListAsync();

        return await dialogues;
    }

    /// <summary>
    /// Get all choices associated with a story node given by ID
    /// </summary>
    
    public async Task<IEnumerable<Choice>> GetAllChoicesOfStoryNode(int id)
    {
        var choices = _db.Choices
                    .Where(choices => choices.StoryNodeId == id)
                    .ToListAsync();

        return await choices;
    }
    
    

    // Ahmed, 11.10 Added GetAllCharactersOfStoryNode method
    public async Task<IEnumerable<Character>> GetAllCharactersOfStoryNode(int id)
    {
        // Query to get charachters in a storynode by Id
        // Characters are found through dialogues in the story node
        // We might need to delink characters from dialogues in the future?
        // so we dont need to go through the dialogues to get the characters
        // but for now, I will just do it like this to avoid changing alot and touch AppContext
        var characters = _db.Dialogues
                    .Where(d => d.StoryNodeId == id)
                    .Select(d => d.Character)
                    .Where(c => c != null)
                    .Distinct()
                    .ToListAsync();
        return await characters;
    }

}