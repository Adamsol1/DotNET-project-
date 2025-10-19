// Controller for handling the home page (start screen) of the game.
// It displays the start view where the player can enter their name to begin a new game.
// The Index action returns a view bound to the StartGameViewModel, which contains the input field for the player's name.
using Microsoft.AspNetCore.Mvc;
using DOTNET_PROJECT.Viewmodels;

namespace DOTNET_PROJECT.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View();
    }
}
