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
    

private readonly IUnitOfWork _uow;

    private readonly ILogger<GameService> _logger;

    // constructor
    public GameService(IUnitOfWork uow, ILogger<GameService> logger)
   
    {
        _uow = uow;
        _logger = logger;
    }
    

    public async Task<StoryNodeDto?> GetNodeAsync(int nodeId)
    {
        var storyNode = await _uow.StoryNodeRepository.GetById(nodeId);
        if (storyNode == null) return null;

        var dialogues = await _uow.StoryNodeRepository.GetAllDialoguesOfStoryNode(storyNode.Id);
        var choices   = await _uow.StoryNodeRepository.GetAllChoicesOfStoryNode(storyNode.Id);

        // (valgfritt) hent karakterer i bulk hvis du viser navn/bilde på replikkene
        // var characterIds = dialogues.Where(d => d.CharacterId != null).Select(d => d.CharacterId!.Value).Distinct().ToList();
        // var characters = await _uow.CharacterRepository.GetByIds(characterIds);

        return new StoryNodeDto
        {
            Id = storyNode.Id,
            Title = storyNode.Title,
            Description = storyNode.Description,
            BackgroundUrl = storyNode.BackgroundUrl,
            Dialogues = dialogues
                .OrderBy(d => d.Order)
                .Select(d => new DialogueDto
                {
                    Id = d.Id,
                    Text = d.Text,
                    CharacterId = d.CharacterId,
                    Order = d.Order
                }).ToList(),
            Choices = choices
                .Select(c => new ChoiceDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    StoryNodeId = c.StoryNodeId,
                    NextStoryNodeId = c.NextStoryNodeId
                }).ToList()
        };
    }

    /// <summary>
    /// Takes the user choice and decied what node to navigate next
    /// </summary>
    /// <param name="currentNodeId">Id of current node</param>
    /// <param name="choiceId">Id of choice</param>
    /// <returns>Id of the next story node</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<int?> ApplyChoiceAsync(int currentNodeId, int choiceId)
    {
        //Gets the choice from the database
        var choice = await _uow.ChoiceRepository.GetById(choiceId);
        //If choice does not exist return null
        if (choice == null) return null;
        //Checks if the choice belongs to the current node
        if (choice.StoryNodeId != currentNodeId)
            throw new InvalidOperationException("Choice does not belong to current node.");

        //Returns the id of the next story node
       
        return choice.NextStoryNodeId; 
    }


}

