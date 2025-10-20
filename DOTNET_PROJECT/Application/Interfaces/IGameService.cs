using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Application.Dtos;

namespace DOTNET_PROJECT.Application.Interfaces;

/*
The idea is to have a single service that handles the game logic.
it will be used to orchestrate the game,
get the choices, check the progress, and get the story.

*/

public interface IGameService
{
    

    // Hent en node med dialoger/valg/karakterer
    Task<StoryNodeDto?> GetNodeAsync(int nodeId);

    // Utfør et valg; verifiser at valget tilhører currentNodeId, returner neste nodeId (null = slutt)
    Task<int?> ApplyChoiceAsync(int currentNodeId, int choiceId);
    
}