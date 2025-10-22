/**
// MVC controller that connects the Razor frontend to the game API.
// It acts as a bridge between the user's actions in the browser and the backend endpoints.
//cd
// Responsibilities:
// • Handles starting a new game by sending the player's name to the API.
// • Fetches and displays the current story node (scene) based on the player ID.
// • Sends the player's choices back to the API to progress the story.
//
// Uses IHttpClientFactory to communicate with the API, which allows easy testing and separation
// between the MVC layer and backend logic. If the API is mocked (MockGameApiController),
// this controller still works seamlessly without a real database.
*/
using Microsoft.AspNetCore.Mvc;
using DOTNET_PROJECT.Viewmodels;
using DOTNET_PROJECT.Application.Interfaces.Services;

namespace DOTNET_PROJECT.Controllers;

// MVC controller that starts on a fixed node and navigates using IGameService
public class GameMvcController : Controller
{
    private readonly IGameService _game;
    public GameMvcController(IGameService game) => _game = game;

    [HttpGet]
    public async Task<IActionResult> Start()
    {
        return RedirectToAction(nameof(Play), new { nodeId = 1 });
    }

    [HttpGet]
    public async Task<IActionResult> Play(int nodeId)
    {
        var node = await _game.GetNodeAsync(nodeId);
        if (node is null)
        {
            TempData["Error"] = $"Story node {nodeId} not found.";
            return RedirectToAction(nameof(Start));
        }

        return View(new PlayViewModel { CurrentNodeId = nodeId, Node = node });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Choice(int nodeId, int choiceId)
    {
        var next = await _game.ApplyChoiceAsync(nodeId, choiceId);
        if (next is null)
        {
            TempData["Info"] = "End of path.";
            return RedirectToAction(nameof(Play), new { nodeId });
        }
        return RedirectToAction(nameof(Play), new { nodeId = next.Value });
    }
}




