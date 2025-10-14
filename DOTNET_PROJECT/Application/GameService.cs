using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Application.Dtos;
using DOTNET_PROJECT.Domain.Models;
using Serilog;


namespace DOTNET_PROJECT.Application;

/*
This service is responsible for handling the game logic.
It will be used to orchestrate the game,
get the choices, check the progress, and get the story.
*/

public class GameService : IGameService
{
    // since the repositories are registered as dependencies
    // in the unit of work, we can just register the unit of work here.

    private readonly IUnitOfWork _uow;

    private readonly ILogger<GameService> _logger;

    // constructor
    public GameService(IUnitOfWork uow, ILogger<GameService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    // start a game. 
    // takes in the user id, and a story node that gets set to 1 in the player character object.
    public async Task<GameSessionDto> StartGame(int userId, string name)
    {
        // validate request eig check if the user exists, or is valid 
        // will need to ajust based on the Error and validation rules Eirik creates. 
        try {
            // start a transaction
            await _uow.BeginAsync();

            // since it is a new game we dont need to check if the user has a player character.
            // we already fetch the characters the user has in another method later. 

            // create the new player object for the user. 
            var playerCharacter = new PlayerCharacter {
                // playerId set by the Db.
                // will need to add an input DTO for this.
                Name = name,
                UserId = userId,
                CurrentStoryNodeId = 1,
                Health = 100,
            };

            // create the object through the player repository.
            await _uow.PlayerCharacterRepository.Create(playerCharacter);

            // save the changes, and commit
            await _uow.SaveAsync();
            await _uow.CommitAsync();

            // return a new game State, since there is no data yet we just return the miniDto
            return new GameSessionDto {
                PlayerCharacterId = playerCharacter.Id,
                UserId = userId,
                StoryNodeId = 1,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

        } catch (Exception ex) {
            // if the try fails, we rollback the transaction. 
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[GameService] Error creating new game for user {userId}", userId);
            throw;
        }
    }

    // load / resume a game.
    public async Task<GameSessionDto> ResumeGame(int userId, int playerCharacterId)
    {
        // again validate the userID, and the playercharacter selected.

        try {
            // start a transaction
            await _uow.BeginAsync();

            var playerCharacter = await _uow.PlayerCharacterRepository.GetById(playerCharacterId);

            if (playerCharacter == null || playerCharacter.UserId != userId) {
                _logger.LogWarning("[Gameservice] Player character with userId {userId} not found", userId);
                throw new Exception("Player character not found");
            }

            // return the player characters game session.
            return new GameSessionDto {
                PlayerCharacterId = playerCharacter.Id,
                UserId = userId,
                StoryNodeId = playerCharacter.CurrentStoryNodeId,
                DateCreated = playerCharacter.DateCreated,
                DateUpdated = playerCharacter.DateUpdated
            };
        } catch (Exception ex) {
            // if the try fails, we rollback the transaction. 
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Gameservice] Error resuming game for userId {userId}", userId);
            throw;
        }
    }

    // save a game
    public async Task<bool> SaveGame(int playerCharacterId)
    {
        // validate request
        try {
            // start a transaction
            await _uow.BeginAsync();

            var playerCharacter = await _uow.PlayerCharacterRepository.GetById(playerCharacterId);

            if (playerCharacter == null) {
                _logger.LogWarning("[Gameservice] Player character with playerCharacterId {playerCharacterId} not found", playerCharacterId);
                return false;
            }

            // save the gamee, auto saved when the CurrentStoryNodeId is updated.
            await _uow.SaveAsync();
            await _uow.CommitAsync();

            return true;
        } catch (Exception ex) {
            // if the try fails, we rollback the transaction. 
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Gameservice] Error saving game for Player character with playerCharacterId {playerCharacterId}", playerCharacterId);
            throw;
        }
    }

    // fetch the game state
    public async Task<MiniGameStateDto> GetGameState(int playerCharacterId)
    {
        try {
            var playerCharacter = await _uow.PlayerCharacterRepository.GetById(playerCharacterId);

            if (playerCharacter == null) {
                _logger.LogWarning("[Gameservice] Player character with playerCharacterId {playerCharacterid} not found", playerCharacterId);
                throw new Exception("Player character not found");
            }

            // return the game state
            return new MiniGameStateDto {
                PlayerCharacterId = playerCharacterId,
                CurrentStoryNodeId = playerCharacter.CurrentStoryNodeId,
                Health = playerCharacter.Health
            };

        } catch (Exception ex) {
            // if the try fails, we rollback the transaction. 
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Gameservice] Error retrieving GameState for Player character with playerCharacterId {playerCharacterid}", playerCharacterId);
            throw;
        }
    }

    // get the story node user is on.
    public async Task<StoryNodeDto> GetCurrentStoryNode(int playerCharacterId)
    {
        try {

            // get the player, might actually just turn get the player into a function.
            var playerCharacter = await _uow.PlayerCharacterRepository.GetById(playerCharacterId);

            if (playerCharacter == null) {
                _logger.LogWarning("[Gameservice] Player character with playerCharacterId {playerCharacterId} not found", playerCharacterId);
                throw new Exception("Player character not found");
            } 

            // get the story node
            var storyNode = await _uow.StoryNodeRepository.GetById(playerCharacter.CurrentStoryNodeId);
            if (storyNode == null)
            {
                _logger.LogWarning("[Gameservice] Story node with ID {StoryNodeId} not found", playerCharacter.CurrentStoryNodeId);
                throw new Exception("Story node not found");
            } 

            var dialogues = await _uow.StoryNodeRepository.GetAllDialoguesOfStoryNode(storyNode.Id);
            var choices = await _uow.StoryNodeRepository.GetAllChoicesOfStoryNode(storyNode.Id);

            // return the full object to the user.

            return new StoryNodeDto {
                Id = storyNode.Id,
                Title = storyNode.Title,
                Description = storyNode.Description,
                BackgroundUrl = storyNode.BackgroundUrl,

                // fetch the dialogues and choices into a list.
                Dialogues = dialogues.Select(d => new DialogueDto {
                    Id = d.Id,
                    Text = d.Text,
                    CharacterId = d.CharacterId,
                    Order = d.Order
                }).ToList(),

                Choices = choices.Select(c => new ChoiceDto {
                    Id = c.Id,
                    Text = c.Text,
                    StoryNodeId = c.StoryNodeId,
                    NextStoryNodeId = c.NextStoryNodeId
                }).ToList()
            };

        } catch (Exception ex) {
            // if the try fails, we rollback the transaction. 
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Gameservice] Error retireving currentStoryNode for Player character with playerCharacterId {playerCharacterId}", playerCharacterId);
            throw;
        }
    }

    // make a choice
    public async Task<MiniGameStateDto> MakeChoice(int playerCharacterId, int choiceId)
    {
        try {
            // start a transaction
            await _uow.BeginAsync();

            // get the player
            var playerCharacter = await _uow.PlayerCharacterRepository.GetById(playerCharacterId);
            if (playerCharacter == null) {
                _logger.LogWarning("[Gameservice] Player character with playerCharacterId {playerCharacterId} not found", playerCharacterId);
                throw new Exception("Player character not found");
            }

            // get the choice
            var choice = await _uow.ChoiceRepository.GetById(choiceId);
            if (choice == null) {
                _logger.LogWarning("[Gameservice] Choice with choiceId {choiceId} not found", choiceId);
                throw new Exception("Choice not found");
            }

            // validate if the choice belongs to the storyNode
            if (choice.StoryNodeId != playerCharacter.CurrentStoryNodeId)
            {
                _logger.LogWarning("[Gameservice] Choice {ChoiceId} does not belong to the current story node {CurrentNodeId} for playerCharacter {PlayerCharacterId}", choiceId, playerCharacter.CurrentStoryNodeId, playerCharacterId);
                throw new Exception("Choice does not belong to the current story node");
            }

            // update the current choice
            playerCharacter.CurrentStoryNodeId = choice.NextStoryNodeId;
            await _uow.PlayerCharacterRepository.Update(playerCharacter);

            // save the changes, and commit
            await _uow.SaveAsync();
            await _uow.CommitAsync();

            // return the game state
            return new MiniGameStateDto {
                PlayerCharacterId = playerCharacterId,
                CurrentStoryNodeId = playerCharacter.CurrentStoryNodeId,
                Health = playerCharacter.Health
            };

        } catch (Exception ex) {
            // if the try fails, we rollback the transaction. 
            await _uow.RollBackAsync();
            _logger.LogWarning(ex, "[Gameservice] Error making choice with choiceId {choiceId}", choiceId);
            throw;
        }

    }

    // get the chartacers game progression data.
    public async Task<GameProgressDto> GetGameProgression(int playerCharacterId)
    {
        try {
            var playerCharacter = await _uow.PlayerCharacterRepository.GetById(playerCharacterId);
            if (playerCharacter == null) {
                _logger.LogWarning("[Gameservice] Player character with playerCharacterId {playerCharacterId} not found", playerCharacterId);
                throw new Exception("Player character not found");
            }

            // get the story node
            var storyNode = await _uow.StoryNodeRepository.GetById(playerCharacter.CurrentStoryNodeId);
            if (storyNode == null) {
                _logger.LogWarning("[Gameservice] Story node with ID {CurrentStoryNodeId} not found", playerCharacter.CurrentStoryNodeId);
                throw new Exception("Story node not found");
            }

            // return the game progression
            return new GameProgressDto {
                PlayerCharacterId = playerCharacterId,
                CurrentStoryNodeId = playerCharacter.CurrentStoryNodeId,
                VisitedStoryNodes = new List<int> { playerCharacter.CurrentStoryNodeId },
                CompletedChoices = new List<int>(),
                TotalChoicesMade = 0
            };

        } catch (Exception ex) {
            _logger.LogError(ex, "[Gameservice] Error retrieving GameProgression for Player character {playerCharacterId}", playerCharacterId);
            throw;
        }
    }

}

