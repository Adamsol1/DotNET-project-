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
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using DOTNET_PROJECT.Viewmodels;
using DOTNET_PROJECT.Application.Dtos;

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
**/

// MVC controller that connects the Razor frontend to the game API.
// Now starts directly on StoryNode 1 (no PlayerCharacter).
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using DOTNET_PROJECT.Viewmodels;
using DOTNET_PROJECT.Application.Dtos;

namespace DOTNET_PROJECT.Controllers;

public class GameMvcController : Controller
{
    private readonly IHttpClientFactory _http;

    // REMOVED: DefaultUserId
    // NEW: always start at StoryNode 1
    private const int DefaultStartNodeId = 1; // CHANGED

    public GameMvcController(IHttpClientFactory http) => _http = http;

    // You can keep this as POST if the home page form already posts here.
    // It no longer uses the player's name; it just starts at node 1.
    [HttpPost]
    public IActionResult Start(StartGameViewModel vm) // CHANGED: no longer validated for name
    {
        // REMOVED: ModelState checks for name, API call to /api/game/start, playerCharacter creation
        return RedirectToAction("Play", new { nodeId = DefaultStartNodeId }); // NEW
    }

    // NEW: Optional convenience GET if you want a route to start without a form
    [HttpGet]
    public IActionResult Start()
        => RedirectToAction("Play", new { nodeId = DefaultStartNodeId }); // NEW

    [HttpGet]
    public async Task<IActionResult> Play(int nodeId) // CHANGED: was (int playerId)
    {
        var client = _http.CreateClient();
        client.BaseAddress ??= new Uri($"{Request.Scheme}://{Request.Host}/");

        // CHANGED: Fetch node directly by id (adjust route to your API)
        var resp = await client.GetAsync($"api/storynodes/{nodeId}");
        if (!resp.IsSuccessStatusCode)
        {
            TempData["Error"] = $"Load failed ({(int)resp.StatusCode}).";
            return RedirectToAction("Index", "Home");
        }

        var json = await resp.Content.ReadAsStringAsync();
        var node = JsonSerializer.Deserialize<StoryNodeDto>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // CHANGED: ViewModel field name
        var model = new PlayViewModel
        {
            CurrentNodeId = nodeId, // CHANGED (was PlayerCharacterId)
            Node = node ?? new StoryNodeDto()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Choice(int nodeId, int choiceId) // CHANGED: was (int playerId, int choiceId)
    {
        var client = _http.CreateClient();
        client.BaseAddress ??= new Uri($"{Request.Scheme}://{Request.Host}/");

        // CHANGED: Post to "next" using fromNodeId + choiceId (adjust if your API differs)
        var payload = new { fromNodeId = nodeId, choiceId };
        var resp = await client.PostAsJsonAsync("api/game/next", payload);
        if (!resp.IsSuccessStatusCode)
        {
            TempData["Error"] = $"Choice failed ({(int)resp.StatusCode}).";
            // On failure, stay on current node
            return RedirectToAction("Play", new { nodeId });
        }

        // Expecting { nextNodeId: int }
        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        var nextNodeId = doc.RootElement.GetProperty("nextNodeId").GetInt32(); // NEW

        return RedirectToAction("Play", new { nodeId = nextNodeId }); // CHANGED
    }
}
