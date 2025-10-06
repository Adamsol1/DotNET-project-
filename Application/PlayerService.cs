using DOTNET_PROJECT.Application.Dtos;
using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Domain.Models;


namespace DOTNET_PROJECT.Application;

public class PlayerService : IPlayerService
{
    private readonly IUnitOfWork _uow;

    // repository
    private readonly IPlayerRepository _playerRepository;

    //services
    IStoryService _storyService;
    IGameService _gameService;

    // constructor
    public PlayerService (
        IUnitOfWork uow,
        IPlayerRepository playerRepository,
        IStoryService storyService,
        IGameService gameService
    )
    {
        _uow = uow;
        _playerRepository = playerRepository;
        _storyRepository = storyRepository;
        _gameRepository = gameRepository;
        _storyService = storyService;
        _gameService = gameService;
    }

    // create a player character
    public async Task<PlayerCharacterDto> CreatePlayerCharacter(CreatePlayerCharacterDto request) {
        // validate request
        // EIRIK

        try {
            // begin transaction
            await _uow.BeginAsync();

            // create player character
            var playerCharacter = new PlayerCharacter {
                Name = request.Name,
                Description = request?.Description,
                CurrentStoryNodeId = request?.CurrentStoryNodeId,
            };

            // add player character to database
            await _playerRepository.AddAsync(playerCharacter);
            await _uow.SaveAsync();

            return returnDto(playerCharacter);

        } catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }

    // get a player character by id
    public async Task<PlayerCharacterDto> GetPlayerCharacterById(int id) {
        // validate request

        try {
            // begin transaction
            await _uow.BeginAsync();

            // get player character by id
            var playerCharacter = await _playerRepository.GetByIdAsync(id);

            // if player character is not found, throw an exception
            if (playerCharacter == null) {
                throw new Exception("Player character not found");
            }

            // return player character
            return returnDto(playerCharacter);
        } catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<PlayerCharacterDto>> GetAllPlayerCharactersByUserId(int userId) {
        // validate request

        try {
            // begin transaction
            await _uow.BeginAsync();

            // get all player characters by user id
            var playerCharacters = await _playerRepository.GetAllByUserIdAsync(userId);

            // if player characters are not found, throw an exception
            if (playerCharacters == null) {
                throw new Exception("Player characters not found");
            }

            // return player characters
            return returnDto(playerCharacters);
        }
        catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteCharacter(int id) {
        // validate request

        try {
            // begin transaction
            await _uow.BeginAsync();

            // check the player character in the database
            var playerCharacter = await _playerRepository.GetByIdAsync(id);

            // if player character is not found, throw an exception
            if (playerCharacter == null) {
                throw new Exception("Player character not found");
            }

            // delete player character
            await _playerRepository.DeleteAsync(playerCharacter);
            await _uow.SaveAsync();

            return true;
        }
        catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }
    
    // get gamestate
    public async Task<GameStateDto> GetGameState(int playerCharacterId) {
        // validate request

        try {
            // begin transaction
            await _uow.BeginAsync();

            // get gamestate from the database
            var gameState = await _playerRepository.GetGameStateAsync(playerCharacterId);
            // if game state is not found, throw an exception
            if (gameState == null) {
                throw new Exception("Game state not found");
            }

            // return game state
            return returnDto(gameState);
        }
        catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }

    // get current story node
    public async Task<StoryNodeDto> GetCurrentStoryNode(int playerCharacterId) {
        // validate request

        try {
            // begin transaction
            await _uow.BeginAsync();

            // get current story node from the database
            var currentStoryNode = await _StoryRepository.GetCurrentStoryNodeAsync(playerCharacterId);
            
            // if current story node is not found, throw an exception
            if (currentStoryNode == null) {
                throw new Exception("Current story node not found");
            }

            // return current story node
            return returnDto(currentStoryNode);
        }
        catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }

    // make a choice
    public async Task<(GameStateDto, StoryNodeDto)> MakeChoice(MakeChoiceDto request) {
        // validate request

        try {
            // begin transaction
            await _uow.BeginAsync();

            var playerCharacter = await _playerRepository.GetByIdAsync(request.PlayerCharacterId);

            if (playerCharacter == null) {
                throw new Exception("Player character not found");
            }

            var nextStoryNode = await _storyRepository.GetByIdAsync(request.NextStoryNodeId);

            return (state, nextStoryNode);
        }
        catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }

    private static PlayerCharacterDto ToDto(PlayerCharacter p) => new PlayerCharacterDto
	{
		Id = p.Id,
		Name = p.Name,
		UserId = p.UserId,
		CurrentNodeId = p.CurrentNodeId
	};
}
