namespace DOTNET_PROJECT.Application.Interfaces;

public interface IPlayerService
{

    // create a player character
    Task<PlayerCharacterDto> CreatePlayerCharacter(CreatePlayerCharacterDto request);

    // get a player character by id
    Task<PlayerCharacterDto> GetPlayerCharacterById(int id);

    // get all player characters belonging to a user
    Task<IEnumerable<PlayerCharacterDto>> GetAllPlayerCharactersByUserId(int userId);

    // delete player character
    Task<bool> DeleteCharacter(int id);

    // game state and choices.
    Task<GameStateDto> GetGameState(int playerCharacterId);

    //get current story node
    Task<StoryNodeDto> GetCurrentStoryNode(int playerCharacterId);

    //make a choice
    Task<(GameStateDto, StoryNodeDto)> MakeChoice(MakeChoiceDto request);
    
}