using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Application.Dtos;
using DOTNET_PROJECT.Domain.Models;
using Serilog;

namespace DOTNET_PROJECT.Application;

public class StoryService : IStoryService
{
    private readonly IUnitOfWork _uow;

    private readonly ILogger<StoryService> _logger;

    // constructor
    public StoryService(IUnitOfWork uow, ILogger<StoryService> logger)
    {
        _logger = logger;
        _uow = uow;
    }

    // get story node by id
    public async Task<StoryNodeDto> GetStoryNodeById(int id) {
        // validate request

        try {
            // begin transaction
            await _uow.BeginAsync();

            // get story node by id
            var storyNode = await _uow.StoryNodeRepository.GetById(id);
            // if story node is not found, throw an exception
            if (storyNode == null) {
                _logger.LogWarning("[Storyservice] StoryNode with id {id} not found", id);
                throw new Exception("Story node not found");
            }

            // and get the dialogues & choices that belongs to the storyNode
            // these methods needs to be created in the repository.
            var dialogues = await _uow.StoryNodeRepository.GetAllDialoguesOfStoryNode(id);
            var choices = await _uow.StoryNodeRepository.GetAllChoicesOfStoryNode(id);

            if (dialogues == null) {
                _logger.LogWarning("[Storyservice] No dialogues for StoryNode with id {StoryNodeId} found", id);
                throw new Exception("Dialogues not found");
            }
            if (choices == null) {
                _logger.LogWarning("[Storyservice] No choices for StoryNode with id {StoryNodeId} found", id);
                throw new Exception("Choices not found");
            }

            // finally, return them to the data transfer object.
            return ReturnStoryNodeDto(storyNode, dialogues, choices);
        
        } catch (Exception ex) {
            // if the try fails, we rollback the transaction. 
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error retrieving StoryNode with id {StoryNodeId}", id);
            throw new Exception("Failed to get story node: " + ex.Message);
        }
    }

    // get all story nodes
    public async Task<IEnumerable<StoryNodeDto>> GetAllStoryNodes() {
        // validate the request

        try {
            // begin transaction
            await _uow.BeginAsync();

            // get all story nodes
            var storyNodes = await _uow.StoryNodeRepository.GetAll();

            if (storyNodes == null) {
                _logger.LogWarning("[Storyservice] StoryNode not found");
                throw new Exception("Story nodes not found");
            }

            // fetch dialoges & choices for each story node and add it to a list.
            var result = new List<StoryNodeDto>();
            foreach (var storyNode in storyNodes) {
                var dialogues = await _uow.StoryNodeRepository.GetAllDialoguesOfStoryNode(storyNode.Id);
                var choices = await _uow.StoryNodeRepository.GetAllChoicesOfStoryNode(storyNode.Id);
                // error handling later
                result.Add(ReturnStoryNodeDto(storyNode, dialogues, choices));
            }

            // since result array contains a return object for each storyNode, 
            // we just return that array
            return result;
        } catch (Exception ex) {
            // rollback, & error handling.
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error retrieving all StoryNodes");
            throw;
        }
    }

    // create a story node
    public async Task<StoryNodeDto> CreateStoryNode(CreateStoryNodeDto request) {
        try {

            // begin transaction
            await _uow.BeginAsync();

            var storyNode = new StoryNode {
                Title = request.Title,
                Description = request.Description,
                BackgroundUrl = request.BackgroundUrl,
            };

            // create in the repository
            await _uow.StoryNodeRepository.Create(storyNode);
            // save and commit
            await _uow.SaveAsync();
            await _uow.CommitAsync();

            // return the story? or should we return a string? for know i just return the storyNode.
            return ReturnStoryNodeDto(storyNode, new List<Dialogue>(), new List<Choice>());

        } catch (Exception ex) {
            // rollback, & error handling.
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error creating StoryNode");
            throw;
        }
    }

    // update a story node
    public async Task<StoryNodeDto> UpdateStoryNode(UpdateStoryNodeDto request) {
        try {
            // begin transaction
            await _uow.BeginAsync();

            var storyNode = await _uow.StoryNodeRepository.GetById(request.Id);

            if (storyNode == null) {
                _logger.LogWarning("[Storyservice] StoryNode with id {StoryNodeId} not found", request.Id);
                throw new Exception("Story node not found");
            }

            storyNode.Title = request.Title;
            storyNode.Description = request.Description;
            storyNode.BackgroundUrl = request.BackgroundUrl;

            // update in the repository
            await _uow.StoryNodeRepository.Update(storyNode);
            // save and commit
            await _uow.SaveAsync();
            await _uow.CommitAsync();

            // get the dialogues & choices that belongs to the storyNode
            var dialogues = await _uow.StoryNodeRepository.GetAllDialoguesOfStoryNode(request.Id);
            var choices = await _uow.StoryNodeRepository.GetAllChoicesOfStoryNode(request.Id);

            // return the storyNode
            return ReturnStoryNodeDto(storyNode, dialogues, choices);
        } catch (Exception ex) {
            // rollback, & error handling.
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error updating StoryNode with id {StoryNodeId}", request.Id);
            throw;
        }
    }

    public async Task<bool> DeleteStoryNode(int id)
    {
        try {
            await _uow.BeginAsync();

            // search in the db for the story node
            var storyNode = await _uow.StoryNodeRepository.GetById(id);
            if (storyNode == null) {
                _logger.LogWarning("[Storyservice] StoryNode with id {StoryNodeId} not found", id);
                throw new Exception("Story node not found");
            }

            // delete the story node
            await _uow.StoryNodeRepository.Delete(id);
            // save and commit
            await _uow.SaveAsync();
            await _uow.CommitAsync();

            return true;
        } catch (Exception ex) {
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error deleting StoryNode with id {StoryNodeId}", id);
            throw;
        }
    }

    // create a dialogue
    public async Task<DialogueDto> CreateDialogue(CreateDialogueDto request)
    {
        try {
            await _uow.BeginAsync();

            // create the dialogue
            var dialogue = new Dialogue {
                Text = request.Text,
                CharacterId = request.CharacterId,
                StoryNodeId = request.StoryNodeId,
                Order = request.Order,
            };

            // create in the repository
            await _uow.DialogueRepository.Create(dialogue);
            // save and commit
            await _uow.SaveAsync();
            await _uow.CommitAsync();

            // return the dialogue
            return ReturnDialogueDto(dialogue);
        }
        catch (Exception ex) {
            // rollback, & error handling.
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error creating Dialogue");
            throw;
        }
    }

    // update a dialogue
    public async Task<DialogueDto> UpdateDialogue(UpdateDialogueDto request) {
        try {
            await _uow.BeginAsync();

            // get the dialogue from the repository
            var dialogue = await _uow.DialogueRepository.GetById(request.Id);
            if (dialogue == null) {
                _logger.LogWarning("[Storyservice] Dialogue with id {DialogueId} not found", request.Id);
                throw new Exception("Dialogue not found");
            }

            // update the dialogue
            dialogue.Id = request.Id;
            dialogue.Text = request.Text;
            dialogue.CharacterId = request.CharacterId;
            dialogue.StoryNodeId = request.StoryNodeId;
            dialogue.Order = request.Order;

            // update in the repository
            await _uow.DialogueRepository.Update(dialogue);
            // save and commit
            await _uow.SaveAsync();
            await _uow.CommitAsync();

            // return the dialogue
            return ReturnDialogueDto(dialogue);
        }
        catch (Exception ex) {
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error updating dialogue with id {DialogueId}", request.Id);
            throw;
        }
    }

    // get the dialogues in a story node
     public async Task<IEnumerable<DialogueDto>> GetDialoguesInStoryNode(int storyNodeId)
    {
        try {
            // begin transaction
            await _uow.BeginAsync();

            // get the dialogues from the repository
            var dialogues = await _uow.StoryNodeRepository.GetAllDialoguesOfStoryNode(storyNodeId);
            return dialogues.Select(ReturnDialogueDto);
        } catch (Exception ex) {
            // rollback, & error handling.
            _logger.LogError(ex, "[Storyservice] Error retrieving dialogues from StoryNode with id {StoryNodeId}", storyNodeId);
            throw;
        }
    }

    // delete a dialogue
    public async Task<bool> DeleteDialogue(int id)
    {
        try {
            await _uow.BeginAsync();
            await _uow.DialogueRepository.Delete(id);
            await _uow.SaveAsync();
            await _uow.CommitAsync();
            return true;
        }
        catch (Exception ex) {
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error deleting dialogue with id {DialogueId}", id);
            throw;
        }
    }

    // create a choice
    public async Task<ChoiceDto> CreateChoice(CreateChoiceDto request)
    {
        try {
            await _uow.BeginAsync();

            // create the choice object
            var choice = new Choice {
                StoryNodeId = request.StoryNodeId,
                NextStoryNodeId = request.NextStoryNodeId,
                Text = request.Text,
            };

            // save to the repository
            await _uow.ChoiceRepository.Create(choice);
            // save and commit
            await _uow.SaveAsync();
            await _uow.CommitAsync();

            // return the choice
            return ReturnChoiceDto(choice);

        }
        catch (Exception ex) {
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error creating choice");
            throw;
        }
    }

    // update a choice
    public async Task<ChoiceDto> UpdateChoice(UpdateChoiceDto request)
    {
        try {
            await _uow.BeginAsync();

            // get the choice from the repository
            var choice = await _uow.ChoiceRepository.GetById(request.Id);
            if (choice == null)
            {
                _logger.LogWarning("[Storyservice] Choice with id {ChoiceId} not found", request.Id);
                throw new Exception("Choice not found");
            }

            // update the choice
            choice.StoryNodeId = request.StoryNodeId;
            choice.NextStoryNodeId = request.NextStoryNodeId;
            choice.Text = request.Text;

            // update in the repository
            await _uow.ChoiceRepository.Update(choice);
            // save and commit
            await _uow.SaveAsync();
            await _uow.CommitAsync();

            // return the choice
            return ReturnChoiceDto(choice);
        }
        catch (Exception ex) {
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error updating choice with id {ChoiceId}", request.Id);
            throw;
        }
    }
    
    // get choices in a story node
    public async Task<IEnumerable<ChoiceDto>> GetChoicesInStoryNode(int storyNodeId)
    {
        try {
            await _uow.BeginAsync();

            // get the choices from the repository
            var choices = await _uow.StoryNodeRepository.GetAllChoicesOfStoryNode(storyNodeId);
            if (choices == null)
            {
                _logger.LogWarning("[Storyservice] Choices with StoryNodeId {StoryNodeId} not found", storyNodeId);
                throw new Exception("Choices not found");
            }
            // return the choices to DTO object
            return choices.Select(ReturnChoiceDto);
        }
        catch (Exception ex) {
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error retrieving choices with StoryNodeId {StoryNodeId}", storyNodeId);
            throw;
        }
    }

    // delete a choice
    public async Task<bool> DeleteChoice(int id)
    {
        try {
            await _uow.BeginAsync();
            await _uow.ChoiceRepository.Delete(id);
            await _uow.SaveAsync();
            await _uow.CommitAsync();
            return true;
        }
        catch (Exception ex) {
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error deleting choice with id {ChoiceId}", id);
            throw;
        }
    }

    // character methods

    // get a character by id
    public async Task<CharacterDto> GetCharacterById(int id)
    {
        try {
            await _uow.BeginAsync();

            // get the character from the repository
            var character = await _uow.CharacterRepository.GetById(id);

            if (character == null)
            {
                _logger.LogWarning("[Storyservice] Character with id {CharacterId} not found", id);
                throw new Exception("Character not found");
            }

            // get the characters dialogues
            var dialogues = await _uow.CharacterRepository.GetAllDialoguesOfCharacter(id);
            if (dialogues == null)
            {
                _logger.LogWarning("[Storyservice] Dialogues with id {DialogueId} not found", id);
                throw new Exception("Dialogues not found");
            }

            // return the character
            return ReturnCharacterDto(character, dialogues);
        }
        catch (Exception ex) {
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error retrieving character with id {CharacterId}", id);
            throw;
        }
    }

    // get all characters
    public async Task<IEnumerable<CharacterDto>> GetAllCharacters()
    {
        try {
            await _uow.BeginAsync();

            var characters = await _uow.CharacterRepository.GetAll();
            if (characters == null)
            {
                _logger.LogWarning("[Storyservice] Characters not found");
                throw new Exception("Characters not found");
            }

            // add the characters to an array
            var result = new List<CharacterDto>();

            // get all the dialogoes for the characters
            foreach (var character in characters) {
                var dialogues = await _uow.CharacterRepository.GetAllDialoguesOfCharacter(character.Id);
                if (dialogues == null)
                {
                    _logger.LogWarning("[Storyservice] Dialogue for character with id {CharacterId} not found", character.Id);
                    throw new Exception("Dialogues not found");
                } 

                result.Add(ReturnCharacterDto(character, dialogues));
            }

            // return the result
            return result;
        }
        catch (Exception ex) {
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error retrieving characters");
            throw;
        }
    }

    // get the characters in a story node
    public async Task<IEnumerable<CharacterDto>> GetCharactersInStoryNode(int storyNodeId)
    {
        try {
            await _uow.BeginAsync();

            // get the characters from the repository
            var characters = await _uow.StoryNodeRepository.GetAllCharactersOfStoryNode(storyNodeId);
            if (characters == null)
            {
                _logger.LogWarning("[Storyservice] Characters from StoryNode with id {StoryNodeId} not found", storyNodeId);
                throw new Exception("Characters not found");
            }

            // add the characters to an array
            var result = new List<CharacterDto>();
            // get all the dialogoes for the characters
            foreach (var character in characters) {
                var dialogues = await _uow.CharacterRepository.GetAllDialoguesOfCharacter(character.Id);
                if (dialogues == null)
                {
                    _logger.LogWarning("[Storyservice] Dialogues from character with id {CharacterId} not found", character.Id);
                    throw new Exception("Dialogues not found");
                }
                
                result.Add(ReturnCharacterDto(character, dialogues));
            }

            // return the result
            return result;
        }
        catch (Exception ex) {
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error retrieving characters from StoryNode with id {StoryNodeId}", storyNodeId);
            throw;
        }
    }

    // create a character
    public async Task<CharacterDto> CreateCharacter(CreateCharacterDto request)
    {
        try {
            await _uow.BeginAsync();

            var character = new Character {
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
            };

            // save to the repository
            await _uow.CharacterRepository.Create(character);
            // save and commit
            await _uow.SaveAsync();
            await _uow.CommitAsync();

            // return the character
            return ReturnCharacterDto(character, new List<Dialogue>());
        }
        catch (Exception ex) {
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error creating character");
            throw;
        }
    }

    // update a character
    public async Task<CharacterDto> UpdateCharacter(UpdateCharacterDto request)
    {
        try {
            await _uow.BeginAsync();

            var character = await _uow.CharacterRepository.GetById(request.Id);
            if (character == null) {
                _logger.LogWarning("[Storyservice] Character with id {CharacterId} not found", request.Id);
                throw new Exception("Character not found");
            }

            character.Name = request.Name;
            character.Description = request.Description;
            character.ImageUrl = request.ImageUrl;

            await _uow.CharacterRepository.Update(character);
            await _uow.SaveAsync();
            await _uow.CommitAsync();

            var dialogues = await _uow.CharacterRepository.GetAllDialoguesOfCharacter(character.Id);
            return ReturnCharacterDto(character, dialogues);
        } catch (Exception ex) {
            await _uow.RollBackAsync();
            _logger.LogError(ex, "[Storyservice] Error updating character with id {CharacterId}", request.Id);
            throw;
        }
    }

    // helper methods

    // return story node dto
     private static StoryNodeDto ReturnStoryNodeDto(StoryNode storyNode, IEnumerable<Dialogue> dialogues, IEnumerable<Choice> choices) => new StoryNodeDto
    {
        Id = storyNode.Id,
        Title = storyNode.Title,
        Description = storyNode.Description,
        BackgroundUrl = storyNode.BackgroundUrl ?? string.Empty,
        Dialogues = dialogues.Select(ReturnDialogueDto).ToList(),
        Choices = choices.Select(ReturnChoiceDto).ToList()
    };

    // return dialogue dto
    private static DialogueDto ReturnDialogueDto(Dialogue dialogue) => new DialogueDto
    {
        Id = dialogue.Id,
        Text = dialogue.Text,
        CharacterId = dialogue.CharacterId,
        StoryNodeId = dialogue.StoryNodeId,
        Order = dialogue.Order
    };

    // return choice dto
    private static ChoiceDto ReturnChoiceDto(Choice choice) => new ChoiceDto
    {
        Id = choice.Id,
        Text = choice.Text,
        StoryNodeId = choice.StoryNodeId,
        NextStoryNodeId = choice.NextStoryNodeId
    };

    // return character dto
    private static CharacterDto ReturnCharacterDto(Character character, IEnumerable<Dialogue> dialogues) => new CharacterDto
    {
        Id = character.Id,
        Name = character.Name,
        Description = character.Description,
        ImageUrl = character.ImageUrl,
        Dialogues = dialogues.Select(ReturnDialogueDto).ToList()
    };
}