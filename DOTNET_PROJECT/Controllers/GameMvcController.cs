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

//TODO: Make comments

/// <summary>
/// Game controller that connects the frontend to the backend.
/// The page is responsible for the user to be able to play the game.
/// Loags the current node, process player choices and the node the choice will lead to from the database. 
/// </summary>
public class GameMvcController : Controller
{
    private readonly IGameService _game;
    /// <summary>
    /// Constructor for the controller
    /// </summary>
 
    public GameMvcController(IGameService game) => _game = game;

    /// <summary>
    /// Starts the game. Will redirect the user to the first story node.
    /// Note : The game does not currently have a save feature. To ensure that the user will always start from the beginning it is hardcoded that the game will start on gamenode with id 1. 
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Start()
    {
       //Redirects to the first story node
        return  RedirectToAction(nameof(Play), new { nodeId= 1});
    }
    /// <summary>
    /// Display for the current story node and its choices. 
    /// </summary>
    /// <param name="nodeId">The id of the node currently used. </param>
    

    [HttpGet]
    public async Task<IActionResult> Play(int nodeId)
    {
        
        var node = await _game.GetNodeAsync(nodeId);
        if (node is null)
        {
            //If the node is not found, the game will restart itself. 
            TempData["Error"] = $"Story node {nodeId} not found.";
            return RedirectToAction(nameof(Start));
        }
        //Creates and returns the viewmodel with the information of the current node
        return View(new PlayViewModel { CurrentNodeId = nodeId, Node = node });
    }

    /// <summary>
    /// Takes in user choice and will move to the next story node. 
    /// </summary>
    /// <param name="nodeId">Id of the current node</param>
    /// <param name="choiceId">Id of the choice the user has taken</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Choice(int nodeId, int choiceId)
    {
        var next = await _game.ApplyChoiceAsync(nodeId, choiceId);
        if (next is null)
        {
            //If the next node is null, it means that the game is over. Informs the user
            TempData["Info"] = "End of path.";
            return RedirectToAction(nameof(Play), new { nodeId });
        }
        //Moves to the next node. 
        return RedirectToAction(nameof(Play), new { nodeId = next.Value });
    }
}



