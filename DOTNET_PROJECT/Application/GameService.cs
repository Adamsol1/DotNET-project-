using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Application.Dtos;
using DOTNET_PROJECT.Domain.Models;


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

    // constructor
    public GameService(IUnitOfWork uow)
    {
        _uow = uow;
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

            // and give the user an error message.
            throw new Exception("Failed to create a new game: " + ex.Message);
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

            // and give the user an error message.
            throw new Exception("Failed to resume game: " + ex.Message);
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
                throw new Exception("Player character not found");
            }

            // save the gamee, auto saved when the CurrentStoryNodeId is updated.
            await _uow.SaveAsync();
            await _uow.CommitAsync();

            return true;
        } catch (Exception ex) {
            // if the try fails, we rollback the transaction. 
            await _uow.RollBackAsync();

            // and give the user an error message.
            throw new Exception("Failed to save game: " + ex.Message);
        }
    }

    // fetch the game state
    public async Task<GameStateDto> GetGameState(int playerCharacterId)
    {
        try {
            var playerCharacter = await _uow.PlayerCharacterRepository.GetById(playerCharacterId);

            if (playerCharacter == null) {
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

            // and give the user an error message.
            throw new Exception("Failed to get game state: " + ex.Message);
        }
    }

    // get the story node user is on.
    public async Task<StoryNodeDto> GetCurrentStoryNode(int playerCharacterId)
    {
        try {

            // get the player, might actually just turn get the player into a function.
            var playerCharacter = await _uow.PlayerCharacterRepository.GetById(playerCharacterId);

            if (playerCharacter == null) {
                throw new Exception("Player character not found");
            }

            // get the story node
            var storyNode = await _uow.StoryNodeRepository.GetById(playerCharacter.CurrentStoryNodeId);
            if (storyNode == null) {
                throw new Exception("Story node not found");
            }

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

            // and give the user an error message.
            throw new Exception("Failed to get current story node: " + ex.Message);
        }
    }

    // make a choice
    public async Task<GameStateDto> MakeChoice(int playerCharacterId, int choiceId)
    {
        try {
            // start a transaction
            await _uow.BeginAsync();

            // get the player
            var playerCharacter = await _uow.PlayerCharacterRepository.GetById(playerCharacterId);
            if (playerCharacter == null) {
                throw new Exception("Player character not found");
            }

            // get the choice
            var choice = await _uow.ChoiceRepository.GetById(choiceId);
            if (choice == null) {
                throw new Exception("Choice not found");
            }

            // validate if the choice belongs to the storyNode
            if (choice.StoryNodeId != playerCharacter.CurrentStoryNodeId) 
            {
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
            throw new Exception("Failed to make choice: " + ex.Message);
        }

    }

    // get the chartacers game progression data.
    public async Task<GameProgressDto> GetGameProgression(int playerCharacterId)
    {
        try {
            var playerCharacter = await _uow.PlayerCharacterRepository.GetById(playerCharacterId);
            if (playerCharacter == null) {
                throw new Exception("Player character not found");
            }

            // get the story node
            var storyNode = await _uow.StoryNodeRepository.GetById(playerCharacter.CurrentStoryNodeId);
            if (storyNode == null) {
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
            throw new Exception("Player character not found");
        }
    }

}

