using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Application.Interfaces.Repositories;
using DOTNET_PROJECT.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DOTNET_PROJECT.Infrastructure.Repositories;


public class DialogueRepository : GenericRepository<Dialogue>, IDialogueRepository
{

    private readonly AppDbContext _db;
    public DialogueRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    /// <summary>
    /// Get the id of the story node where dialogue is shown
    /// The method expects either one or zero results because the storynode ID is unique.
    /// </summary>

    public async Task<int> GetStoryNodeId(int id)
    {
        var storyNodeId = _db.Dialogues
                    .Where(dialogues => dialogues.Id == id)
                    .Select(dialogues => dialogues.StoryNodeId)
                    .SingleOrDefaultAsync();

        return await storyNodeId;
    }

        /// <summary>
        /// Get the StoryNode this dialogue belongs to
        /// The method expects either one or zero results because the storynode ID is unique.
        /// </summary>

        public async Task<StoryNode?> GetStoryNode(int id)
        {
            var storyNode = _db.Dialogues
                        .Where(dialogues => dialogues.Id == id)
                        .Select(dialogues => dialogues.StoryNode)
                        .SingleOrDefaultAsync();

            return await storyNode;
        }

    /// <summary>
    /// Get the order the dialogues is shown in current story node
    /// The method expects either one or zero results because the dialogue should only be used once per story node.
    /// </summary>

    public async Task<int> GetDialogueOrder(int id)
    {
        var dialogueOrder = _db.Dialogues
                    .Where(dialogues => dialogues.Id == id)
                    .Select(dialogues => dialogues.Order)
                    .SingleOrDefaultAsync();

        return await dialogueOrder;
    }


    /// <summary>
    /// Get the character id of the character speaking the dialogue
    /// </summary>

    public async Task<int> GetCharacterId(int id)
    {
        var characterId = _db.Dialogues
                    .Where(dialogues => dialogues.Id == id)
                    .Select(dialogues => dialogues.CharacterId)
                    .SingleOrDefaultAsync();

        return await characterId;
    }

    /// <summary>
    /// Get the character speaking the dialogue
    /// The method expects either one or zero results because not all dialogues have a character. For example, narration and sound effects.
    /// </summary>

    public async Task<Character?> GetCharacter(int id)
    {
        var character = _db.Dialogues
                    .Where(dialogues => dialogues.Id == id)
                    .Select(dialogues => dialogues.Character)
                    .SingleOrDefaultAsync();

        return await character;
    }

    /// <summary>
    /// Get the dialogues text
    /// The method expects either one or zero results because the dialogue ID is unique.
    /// </summary>
    
    
    public async Task<string> GetDialogueText(int id)
    {
        var dialogueText = _db.Dialogues
                    .Where(dialogues => dialogues.Id == id)
                    .Select(dialogues => dialogues.Text)
                    .SingleOrDefaultAsync();

        return await dialogueText;
    }
}