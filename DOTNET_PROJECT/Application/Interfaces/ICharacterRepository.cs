


using DOTNET_PROJECT.Domain.Models;

namespace DOTNET_PROJECT.Application.Interfaces;


public interface CharacterRepository : IGenericRepository<Character>
{
    /// <summary>
    /// Get character name with ID
    /// </summary>
    
   
    Task<String> GetCharacterNameById(int id);
    /// <summary>
    /// Get a character associated with given name
    /// </summary>
 
    Task<Character> GetCharacterByName(string name);
    /// <summary>
    /// Get all characters associated with given name
    /// </summary>
    
    Task<IEnumerable<Character>> GetAllCharactersByName(string name);
    /// <summary>
    /// Get description of the character given by ID
    /// </summary>
    
    Task<String> GetCharacterDescription(int id);
    /// <summary>
    /// Get the URL of the character image given by ID
    /// </summary>
    
    Task<String> GetCharacterImageUrl(int id);
    /// <summary>
    /// Get all dialogues associated with a character given by ID
    /// </summary>
   
    Task<IEnumerable<Dialogue>> GetAllDialoguesOfCharacter(int id);



    
}