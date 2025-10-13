using Microsoft.AspNetCore.Mvc;
using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Application.Dtos;

namespace DOTNET_PROJECT.Controllers;

// route = api/game as it removed the controller from GameController
[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    // we inject the GameController as it only need access to that
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    // start game, can route to a button that creates the game instance. and saves it to the user.
    // remeber to create Request DTOS 
    [HttpPost("start/{userId}")]
    public async Task<ActionResult<GameSessionDto>> StartGame(int userId, [FromBody] string name)
    {
        try
        {

            // weak validation for now. 
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Player name is required");
            }

            // start game by delegating to the service. 
            var gameSession = await _gameService.StartGame(userId, name);

            // if the response is ok a gameSession will start. 
            return Ok(gameSession);
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to start game: {ex.Message}");
        }
    }
    

    // resume and fetch an existing game
    [HttpPost("resume/{userId}/{playerCharacterId}")]
    public async Task<ActionResult<GameSessionDto>> ResumeGame(int userId, int playerCharacterId)
    {
        try {
            // call the service controller to get the game.
            var gameSession = await _gameService.ResumeGame(userId, playerCharacterId);
            if (gameSession = null) return Exception("Could not find a game");

            // return the game session. 
            return Ok(gameSession);
        } catch (Exception ex)
        {
            return BadRequest($"Failed to resume game: {ex.Message}");
        }
    }

    //save a game. 
    [HttpPost("save/{playerCharacterId}")]
    public async Task<ActionResult<bool>> SaveGame(int playerCharacterId)
    {
        try
        {
            // save the game through the service. 
            var result = await _gameService.SaveGame(playerCharacterId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // make a choice. 
    [HttpPost("choice/{playerCharacterId}/{choiceId}")]
    public async Task<ActionResult<MiniGameStateDto>> MakeChoice(int playerCharacterId, int choiceId)
    {
        try
        {
            // call on the gameService method to make a choice, by passing the id from the frontEnd 
            // based on the choices sent int eh StoryService & GameService. 
            var gameState = await _gameService.MakeChoice(playerCharacterId, choiceId);
            return Ok(gameState);
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to make choice: {ex.Message}");
        }
    }
    
    [HttpGet("state/{playerCharacterId}")]
    public async Task<ActionResult<MiniGameStateDto>> GetGameState(int playerCharacterId)
    {
        try {
            // fet the game state from service method.
            var  gameState = await _gameService.GetGameState(playerCharacterId);

            return Ok(gameState);
        } 
        catch ( Exception ex)
        {
            return BadRequest($"Failed to get game state: {ex.Message}");

        }
    }

    // get the story node with coices and dialoges. 
    [HttpGet("story/{playerCharacterId}")]
    public async Task<ActionResult<StoryNodeDto>> GetCurrentStoryNode(int playerCharacterId)
    {
        try
        {
            var storyNode = await _gameService.GetCurrentStoryNode(playerCharacterId);
            return Ok(storyNode);
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to get current story node: {ex.Message}");
        }
    }

}