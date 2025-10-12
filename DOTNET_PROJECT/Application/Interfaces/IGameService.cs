using DOTNET_PROJECT.Domain.Models;

namespace DOTNET_PROJECT.Application.Interfaces;

/*
The idea is to have a single service that handles the game logic.
it will be used to orchestrate the game,
get the choices, check the progress, and get the story.

*/

public interface IGameService
{
    // Changed the structure of this file to be more organized 9.10

    // GAME SESSION MANAGEMENTS. 

    // create / start a game - starts the game at the default node 1. 
    Task<GameSessionDto> StartGame(int userId, string name);
    
    // load / resume a game,
    Task<GameSessionDto> ResumeGame(int userId, int playerCharacterId);

    // Save a Game
    Task<bool> SaveGame(int playerCharacterId);

    // GAME FLOW / GAME STATE MANAGEMENTS. 

    // get the current gameState
    Task<MiniGameStateDto> GetGameState(int playerCharacterId);

    // Get the current story node player is on.
    Task<StoryNodeDto> GetCurrentStoryNode(int playerCharacterId);

    // Make a choice
    Task<MiniGameStateDto> MakeChoice(int playerCharacterId, int choiceId);



    // GAME PROGRESSION MANAGEMENTS, for the future. 
    Task<GameProgressDto> GetGameProgression(int playerCharacterId);

    // Check if the player can make a choice
    Task<bool> CanMakeChoice(int playerCharacterId, int choiceId);
}