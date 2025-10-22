using Microsoft.AspNetCore.Mvc;
using DOTNET_PROJECT.Application.Interfaces.Repositories;
using DOTNET_PROJECT.Application.Interfaces.Services;
using DOTNET_PROJECT.Application.Dtos;

namespace DOTNET_PROJECT.Controllers;


[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IPlayerService _playerService;

    public GameController(IGameService gameService, IPlayerService playerService)
    {
        _gameService = gameService;
        _playerService = playerService;
    }

    // start a new game from scratch.
    // takes inn a request from the frontend to start a new game.
    [HttpPost("start")]
        public async Task<ActionResult<GameSaveDto>> Start(StartGameRequestDto request)
    {
        try {
            // create a new game save.
            var gameSave = await _gameService.CreateGame(request.UserId, request.SaveName);

            if (gameSave == null) {
                return BadRequest("Failed to create game save");
            }

            // return the game save.
            return Ok(gameSave);
        } catch (Exception ex) {
            return BadRequest($"Failed to start game");
        }
    }

    // load a game save.
    [HttpGet("load/{saveId}")]
    public async Task<ActionResult<GameSaveDto>> LoadGame(int saveId)
    {
        try {
            // load the game save.
            var gameSave = await _gameService.GetGameSave(saveId);
            return Ok(gameSave);
        } catch (Exception ex) {
            return BadRequest($"Failed to load game");
        }
    }

    // Make a choice
    [HttpPost("choice")]
        public async Task<ActionResult<GameSaveDto>> MakeChoice(MakeChoiceRequestDto request)
    {
        try {
            // make the choice.
            var gameSave = await _gameService.MakeChoice(request.SaveId, request.ChoiceId);
            
            return Ok(gameSave);
        } catch (Exception ex) {
            return BadRequest($"Failed to make choice");
        }
    }

    // Get user's saved games
    [HttpGet("saves/{userId}")]
    public async Task<ActionResult<IEnumerable<GameSaveDto>>> GetUserSaves(int userId)
    {
        try {
            // get the user's saved games.
            var gameSaves = await _gameService.GetUserGameSaves(userId);
            return Ok(gameSaves);
        } catch (Exception ex) {
            return BadRequest($"Failed to get user's saved games: {ex.Message}");
        }
    }

    // Delete a game save
    [HttpDelete("delete/{saveId}")]
    public async Task<ActionResult<bool>> DeleteGameSave(int saveId)
    {
        try {
            // delete the game save.
            var result = await _gameService.DeleteGameSave(saveId);

            if(!result) {
                return NotFound("Failed to delete game save");
            }

            return Ok(new { message = "Save deleted successfully" });

        } catch (Exception ex) {
            return BadRequest($"Failed to delete game save");
        }
    }

    // Update a game save
    [HttpPut("update/{saveId}")]
    public async Task<ActionResult<GameSaveDto>> UpdateGameSave(int saveId, UpdateGameSaveRequest request)
    {
        try {
            // update the game save.
            var gameSave = await _gameService.UpdateGameSave(saveId, request.CurrentStoryNodeId);
            return Ok(gameSave);
        } catch (Exception ex) {
            return BadRequest($"Failed to update game save");
        }
    }

}