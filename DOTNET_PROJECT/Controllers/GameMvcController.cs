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
using System.Net.Http.Json;
using System.Text.Json;
using DOTNET_PROJECT.Viewmodels;
using DOTNET_PROJECT.Application.Dtos;
using DOTNET_PROJECT.Application.Interfaces;


/**
namespace DOTNET_PROJECT.Controllers;

public class GameMvcController : Controller
{
    private readonly IHttpClientFactory _http;
    private const int DefaultUserId = 1;

    public GameMvcController(IHttpClientFactory http) => _http = http;

    [HttpPost]
    public async Task<IActionResult> Start(StartGameViewModel vm)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(vm.Name))
        {
            TempData["Error"] = "Please enter a name.";
            return RedirectToAction("Index", "Home");
        }

        var client = _http.CreateClient();
        client.BaseAddress ??= new Uri($"{Request.Scheme}://{Request.Host}/");

        var payload = new { userId = DefaultUserId, name = vm.Name };
        var resp = await client.PostAsJsonAsync("api/game/start", payload);
        if (!resp.IsSuccessStatusCode)
        {
            TempData["Error"] = $"Start failed ({(int)resp.StatusCode}).";
            return RedirectToAction("Index", "Home");
        }

        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        var playerId = doc.RootElement.GetProperty("playerCharacterId").GetInt32();

        return RedirectToAction("Play", new { playerId });
    }

    [HttpGet]
    public async Task<IActionResult> Play(int playerId)
    {
        var client = _http.CreateClient();
        client.BaseAddress ??= new Uri($"{Request.Scheme}://{Request.Host}/");

        var resp = await client.GetAsync($"api/game/current?playerId={playerId}");
        if (!resp.IsSuccessStatusCode)
        {
            TempData["Error"] = $"Load failed ({(int)resp.StatusCode}).";
            return RedirectToAction("Index", "Home");
        }

        var json = await resp.Content.ReadAsStringAsync();
        var node = System.Text.Json.JsonSerializer.Deserialize<StoryNodeDto>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        var model = new PlayViewModel { PlayerCharacterId = playerId, Node = node ?? new StoryNodeDto() };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Choice(int playerId, int choiceId)
    {
        var client = _http.CreateClient();
        client.BaseAddress ??= new Uri($"{Request.Scheme}://{Request.Host}/");

        var resp = await client.PostAsJsonAsync("api/game/choice", new { playerId, choiceId });
        if (!resp.IsSuccessStatusCode)
        {
            TempData["Error"] = $"Choice failed ({(int)resp.StatusCode}).";
        }
        return RedirectToAction("Play", new { playerId });
    }
}


// MVC controller that connects the Razor frontend to the game API.
// Now starts directly on StoryNode 1 (no PlayerCharacter).

public class GameMvcController : Controller
{
    private readonly IGameService _game;
    public GameMvcController(IGameService game) => _game = game;

    [HttpGet]
    public async Task<IActionResult> Start()
    {
        var startId = await _game.GetStartNodeIdAsync();
        return RedirectToAction(nameof(Play), new { nodeId = startId });
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
            // slutt – vis samme node eller send til “game over”/summary
            TempData["Info"] = "End of path.";
            return RedirectToAction(nameof(Play), new { nodeId });
        }
        return RedirectToAction(nameof(Play), new { nodeId = next.Value });
    }
}
**/

//TODO: Make comments

public class GameMvcController : Controller
{
    private readonly IGameService _game;
    public GameMvcController(IGameService game) => _game = game;

    [HttpGet]
    public async Task<IActionResult> Start()
    {
        return  RedirectToAction(nameof(Play), new { nodeId= 1});
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
            // slutt – vis samme node eller send til “game over”/summary
            TempData["Info"] = "End of path.";
            return RedirectToAction(nameof(Play), new { nodeId });
        }
        return RedirectToAction(nameof(Play), new { nodeId = next.Value }); //TODO: check is nextstorynode Exists
    }
}



