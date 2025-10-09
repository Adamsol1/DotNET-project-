using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DOTNET_PROJECT.Infrastructure.Repositories;

public class CharacterRepository : GenericRepository<Character>, ICharacterRepository
{

    private readonly AppDbContext _db;
    public CharacterRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    /// <summary>
    /// Get a character associated with given name
    /// The method will return the first character found with the given name.
    /// </summary>

    public async Task<Character> GetCharacterByName(string name)
    {
        /// Query to get a character by name
        var character = _db.Characters
                    .Where(Characters => Characters.Name == name)
                    .FirstOrDefaultAsync();
        return await character;
    }


    /// <summary>
    /// Get character name with id
    /// The method expects either one or zero results because the character name is unique.
    /// </summary>

    public async Task<string> GetCharacterNameById(int id)
    {
        /// Query to get character name by id 
        var characterName = _db.Characters
                    .Where(Characters => Characters.Id == id)
                    .Select(Characters => Characters.Name)
                    .SingleOrDefaultAsync();

        return await characterName;

    }



    /// <summary>
    /// Get all characters associated with given name
    /// </summary>

    public async Task<IEnumerable<Character>> GetAllCharactersWithName(string name)
    {
        /// Query to get all characters associated with given name in list
        var characters = await _db.Characters
                    .Where(Characters => Characters.Name == name)
                    .ToListAsync();

        return characters;
    }


    /// <summary>
    /// Get description of the character given by ID
    /// The method expects either one or zero results because the character description is optional.
    /// </summary>

    public async Task<string> GetCharacterDescription(int id)
    {
        /// Query to get character description by id

        var characterDescription = _db.Characters
                    .Where(Characters => Characters.Id == id)
                    .Select(Characters => Characters.Description)
                    .SingleOrDefaultAsync();

        return await characterDescription;
    }


    /// <summary>
    /// Get the URL of the character image given by ID
    /// The method expects either one or zero results because the character image is optional.
    /// </summary>

    public async Task<string> GetCharacterImageUrl(int id)
    {
        /// Query to get character image url by id
        var imageUrl = _db.Characters
                    .Where(Characters => Characters.Id == id)
                    .Select(Characters => Characters.ImageUrl)
                    .SingleOrDefaultAsync();

        return await imageUrl;
    }


    /// <summary>
    /// Get all dialogues associated with a character given by ID
    /// </summary>


    public async Task<IEnumerable<Dialogue>> GetAllDialoguesOfCharacter(int id)
    {
        /// Query to get all dialogues associated with a character given by ID
        var dialogues = await _db.Dialogues
                    .Where(characters => characters.CharacterId == id)
                    .ToListAsync();
        
        return dialogues;
    }
}
