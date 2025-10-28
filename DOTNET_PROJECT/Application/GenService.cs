using DOTNET_PROJECT.Application.Interfaces.Repositories;
using DOTNET_PROJECT.Application.Interfaces.Services;
using DOTNET_PROJECT.Application.Dtos;
using Microsoft.EntityFrameworkCore;
using DOTNET_PROJECT.Domain.Models;
using System.Threading.Tasks;
using System;


namespace DOTNET_PROJECT.Application;

public class GenService : IGenService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<GenService> _logger;

    public GenService(IUnitOfWork uow, ILogger<GenService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    #region Execution methods.

    // excutes an operation and returns a result.
    // used for operations where we can pass in an an function method to execute.
    // and delegate the work to the caller.
    public async Task<T> Execute<T>(Func<Task<T>> request)
    {
        // start a transaction to avoid aloways doing this in each crud service function.
        await _uow.BeginAsync();

        try {
            // execute the request.
            var result = await request();
            // save and commit the transaction.
            await _uow.SaveAsync();
            // commit the transaction.
            await _uow.CommitAsync();
            return result;

        } catch (Exception ex) {
            // if the try fails, we rollback the transaction.
            await _uow.RollBackAsync();
            // and give the user an error message.
            _logger.LogError(ex, "GenService: Error executing operation");
            // throw the exception to the caller.
            // since we dont know what kind of exception it is, we just throw it
            // and let the caller handle it.
            throw;
        }
    }

    // this counterpart method does the same ex
    public async Task Execute(Func<Task> request)
    {
        await _uow.BeginAsync();
        try
        {
            await request();
            await _uow.SaveAsync();
            await _uow.CommitAsync();
        }
        catch (Exception ex)
        {
            await _uow.RollBackAsync();
            _logger.LogError(ex, "GenService: Error executing transaction operation");
            throw;
        }
    }

    #endregion

    #region DTO mapper methods.

    // map storyNode, dialogue and choice arrays to a StoryNodeDto.
    // to avoid repeating this in each service. that might use it.
    public StoryNodeDto MapStoryNode( StoryNode storyNode, IEnumerable<Dialogue> dialogues, IEnumerable<Choice> choices)
    {
        return new StoryNodeDto
        {
            Id = storyNode.Id,
            Title = storyNode.Title,
            Description = storyNode.Description,
            BackgroundUrl = storyNode.BackgroundUrl ?? string.Empty,

            // dialogues and choices where we loop through and map those into their counterpart dtos.
            Dialogues = dialogues.OrderBy(d => d.Order)
            .Select(MapDialogue)
            .ToList(),
            
            Choices = choices.Select(MapChoice)
            .ToList()
        };
    }

    // map choice to a ChoiceDto.
    public ChoiceDto MapChoice(Choice choice)
    {
        return new ChoiceDto
        {
            Id = choice.Id,
            Text = choice.Text,
            StoryNodeId = choice.StoryNodeId,
            NextStoryNodeId = choice.NextStoryNodeId
        };
    }

    // map dialogue to a DialogueDto.
    public DialogueDto MapDialogue(Dialogue dialogue)
    {
        return new DialogueDto
        {
            Id = dialogue.Id,
            Text = dialogue.Text,
            CharacterId = dialogue.CharacterId,
            StoryNodeId = dialogue.StoryNodeId,
            Order = dialogue.Order
        };
    }

    // map character to a CharacterDto.
    public CharacterDto MapCharacter(Character character, IEnumerable<Dialogue> dialogues)
    {
        return new CharacterDto
        {
            Id = character.Id,
            Name = character.Name,
            Description = character.Description,
            ImageUrl = character.ImageUrl,
            Dialogues = dialogues.Select(MapDialogue).ToList()
        };
    }

    // map gameSave to a GameSaveDto.
    public GameSaveDto MapGameSave(GameSave gameSave)
    {
        return new GameSaveDto
        {
            Id = gameSave.Id,
            UserId = gameSave.UserId,
            SaveName = gameSave.SaveName,
            PlayerCharacterId = gameSave.PlayerCharacterId,
            CurrentStoryNodeId = gameSave.CurrentStoryNodeId,
            LastUpdate = gameSave.LastUpdate
        };
    }

    #endregion

    #region Validation methods.
    // The validation methods are used to validate data and comes
    // with checks prebuilt, this is to create a common validation handler. 

    // function that checks if an entity exists by id.
    public async Task<T> ValidateEntityExists<T>(int id) where T : class
    {
        // the getRepository method fetches the repository we are looking for.
        var entity = await _uow.GetRepository<T>().GetById(id);

        if (entity == null) {
            var entityName = typeof(T).Name;
            _logger.LogWarning("GenService: {EntityName} with id {Id} not found");
            throw new KeyNotFoundException($"{entityName} with id {id} not found");
        }

        return entity;
    }

    // function that checks if a choice belongs to a node.
    public async Task<bool> CheckChoiceInNode(int choiceId, int nodeId)
    {
        // get the choice from the repository.
        var choice = await _uow.ChoiceRepository.GetById(choiceId);
        Console.WriteLine("Genservice node used is: " + nodeId );
        Console.WriteLine("GenService: Choise id: " + choiceId);
        Console.WriteLine("GenService: Choise Stroy node: " + choice.StoryNodeId);

        if (choice == null) {
            _logger.LogWarning("GenService: Choice with id {ChoiceId} not found");
            // we dont really need to throw exception here, since we are just checking if it exists.
            return false;
        }
    /*
        if (choice.StoryNodeId != nodeId) {
            _logger.LogWarning("GenService: Choice with id {ChoiceId} does not belong to node with id {NodeId}");
            return false;
        }
*/

        // if the checks pass, return true.
        return true;
    }

    public async Task<bool> CheckStoryNodeExists(int nodeId)
    {
        var storyNode = await _uow.StoryNodeRepository.GetById(nodeId);

        if (storyNode == null) {
            _logger.LogWarning("GenService: StoryNode with id {NodeId} not found");
            return false;
        }

        return true;
    }

    #endregion

    #region common methods.



    #endregion
}