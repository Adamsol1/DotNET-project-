using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Application.Dtos;
using DOTNET_PROJECT.Domain.Models;

namespace DOTNET_PROJECT.Application;

public class StoryService : IStoryService
{
    private readonly IUnitOfWork _uow;

    // constructor
    public StoryService(IUnitOfWork uow)
    {
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
                throw new Exception("Story node not found");
            }

            // and get the dialogues & choices that belongs to the storyNode
            // these methods needs to be created in the repository.
            var dialogues = await _uow.StoryNodeRepository.GetAllDialoguesOfStoryNode(id);
            var choices = await _uow.StoryNodeRepository.GetAllChoicesOfStoryNode(id);

            if (dialogues == null) {
                throw new Exception("Dialogues not found");
            }
            if (choices == null) {
                throw new Exception("Choices not found");
            }

            // finally, return them to the data transfer object.
            return ReturnStoryNodeDto(storyNode, dialogues, choices);
        
        } catch (Exception ex) {
            // if the try fails, we rollback the transaction. 
            await _uow.RollBackAsync();

            // and give the user an error message.
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
            throw new Exception("Failed to get all story nodes: " + ex.Message);
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
            throw new Exception("Failed to create story node: " + ex.Message);
        }
    }

    // update a story node
    public async Task<StoryNodeDto> UpdateStoryNode(UpdateStoryNodeDto request) {
        try {
            // begin transaction
            await _uow.BeginAsync();

            var storyNode = await _uow.StoryNodeRepository.GetById(request.Id);

            if (storyNode == null) {
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
            throw new Exception("Failed to update story node: " + ex.Message);
        }
    }

    public async Task<bool> DeleteStoryNode(int id)
    {
        try {
            await _uow.BeginAsync();

            // search in the db for the story node
            var storyNode = await _uow.StoryNodeRepository.GetById(id);
            if (storyNode == null) {
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
            throw new Exception($"Failed to delete story node: {ex.Message}");
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
            throw new Exception($"Failed to create dialogue: {ex.Message}");
        }
    }

    // update a dialogue
    public async Task<DialogueDto> UpdateDialogue(UpdateDialogueDto request) {
        try {
            await _uow.BeginAsync();

            // get the dialogue from the repository
            var dialogue = await _uow.DialogueRepository.GetById(request.Id);
            if (dialogue == null) {
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
            throw new Exception($"Failed to update dialogue: {ex.Message}");
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
            throw new Exception($"Failed to get dialogues: {ex.Message}");
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
            throw new Exception($"Failed to delete dialogue: {ex.Message}");
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
            throw new Exception($"Failed to create choice: {ex.Message}");
        }
    }

    // update a choice
    public async Task<ChoiceDto> UpdateChoice(UpdateChoiceDto request)
    {
        try {
            await _uow.BeginAsync();

            // get the choice from the repository
            var choice = await _uow.ChoiceRepository.GetById(request.Id);
            if (choice == null) throw new Exception("Choice not found");

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
            throw new Exception($"Failed to update choice: {ex.Message}");
        }
    }
    
    // get choices in a story node
    public async Task<IEnumerable<ChoiceDto>> GetChoicesInStoryNode(int storyNodeId)
    {
        try {
            await _uow.BeginAsync();

            // get the choices from the repository
            var choices = await _uow.StoryNodeRepository.GetAllChoicesOfStoryNode(storyNodeId);
            if (choices == null) throw new Exception("Choices not found");

            // return the choices to DTO object
            return choices.Select(ReturnChoiceDto);
        }
        catch (Exception ex) {
            await _uow.RollBackAsync();
            throw new Exception($"Failed to get choices: {ex.Message}");
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
            throw new Exception($"Failed to delete choice: {ex.Message}");
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

            if (character == null) throw new Exception("Character not found");

            // get the characters dialogues
            var dialogues = await _uow.CharacterRepository.GetAllDialoguesOfCharacter(id);
            if (dialogues == null) throw new Exception("Dialogues not found");

            // return the character
            return ReturnCharacterDto(character, dialogues);
        }
        catch (Exception ex) {
            await _uow.RollBackAsync();
            throw new Exception($"Failed to get character: {ex.Message}");
        }
    }

    // get all characters
    public async Task<IEnumerable<CharacterDto>> GetAllCharacters()
    {
        try {
            await _uow.BeginAsync();

            var characters = await _uow.CharacterRepository.GetAll();
            if (characters == null) throw new Exception("Characters not found");

            // add the characters to an array
            var result = new List<CharacterDto>();

            // get all the dialogoes for the characters
            foreach (var character in characters) {
                var dialogues = await _uow.CharacterRepository.GetAllDialoguesOfCharacter(character.Id);
                if (dialogues == null) throw new Exception("Dialogues not found");

                result.Add(ReturnCharacterDto(character, dialogues));
            }

            // return the result
            return result;
        }
        catch (Exception ex) {
            await _uow.RollBackAsync();
            throw new Exception($"Failed to get characters: {ex.Message}");
        }
    }

    // get the characters in a story node
    public async Task<IEnumerable<CharacterDto>> GetCharactersInStoryNode(int storyNodeId)
    {
        try {
            await _uow.BeginAsync();

            // get the characters from the repository
            var characters = await _uow.StoryNodeRepository.GetAllCharactersOfStoryNode(storyNodeId);
            if (characters == null) throw new Exception("Characters not found");

            // add the characters to an array
            var result = new List<CharacterDto>();
            // get all the dialogoes for the characters
            foreach (var character in characters) {
                var dialogues = await _uow.CharacterRepository.GetAllDialoguesOfCharacter(character.Id);
                if (dialogues == null) throw new Exception("Dialogues not found");
                
                result.Add(ReturnCharacterDto(character, dialogues));
            }

            // return the result
            return result;
        }
        catch (Exception ex) {
            await _uow.RollBackAsync();
            throw new Exception($"Failed to get characters: {ex.Message}");
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
            throw new Exception($"Failed to create character: {ex.Message}");
        }
    }

    // update a character
    public async Task<CharacterDto> UpdateCharacter(UpdateCharacterDto request)
    {
        try {
            await _uow.BeginAsync();

            var character = await _uow.CharacterRepository.GetById(request.Id);
            if (character == null) {
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
            throw new Exception($"Failed to update character: {ex.Message}");
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