using DOTNET_PROJECT.Domain.Models;

namespace DOTNET_PROJECT.Application.Interfaces;

/*
The idea is to have a single service that handles the game logic.
it will be usedd to create a game, 
get the choices, check the progress, and get the story.

*/

public interface IGameService
{
    // create/ start a game
    Task<GameStateDto> StartGame(int userId, int storyNodeId);

    // get the current game state
    task<GameStateDto> GetGameState(int userId);

    // save game state
    Task<GameStateDto> SaveGameState(int userId, int storyNodeId);

    // move to next story node
    Task<GameStateDto> MoveToNextNode(int userId, int storyNodeId);
    
    // move to previous story node
    Task<GameStateDto> MoveToPreviousNode(int userId, int storyNodeId);

    // get progression
}