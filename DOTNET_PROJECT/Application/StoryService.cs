using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Domain.Models;

namespace DOTNET_PROJECT.Application;

public class StoryService : IStoryService
{
    private readonly IUnitOfWork _uow;

    // repository
    private readonly IStoryRepository _storyRepository;

    // constructor
    public StoryService(IUnitOfWork uow, IStoryRepository storyRepository, IDialogueRepository dialogueRepository, IChoiceRepository choiceRepository, ICharacterRepository characterRepository)
    {
        _uow = uow;
        _storyRepository = storyRepository;
        _dialogueRepository = dialogueRepository;
        _choiceRepository = choiceRepository;
        _characterRepository = characterRepository;
    }

    // get story node by id
    public async Task<StoryNodeDto> GetStoryNodeById(int id) {
        // validate request

        try {
            // begin transaction
            await _uow.BeginAsync();

            // get story node by id
            var storyNode = await _storyRepository.GetByIdAsync(id);
            // if story node is not found, throw an exception
            if (storyNode == null) {
                throw new Exception("Story node not found");
            }
        } catch (Exception ex) {
            throw new Exception(ex.Message);
        }

        // return story node
        return returnDto(storyNode);
    }
}