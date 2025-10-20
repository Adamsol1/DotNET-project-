// ViewModel for the game play page.
// It passes the player's character ID and the current story node (scene)
// from the controller to the Razor view, so the view can render
// the correct story text, background, and available choices.
using DOTNET_PROJECT.Application.Dtos;

namespace DOTNET_PROJECT.Viewmodels
{
 /// <summary>
 /// Viewmodel used for the game.
 /// Contains the current node and its id. This allows the page to load in background, dialogue etc. 
 /// </summary>
    public class PlayViewModel
    {
        /// <summary>
        /// Id of the current story node
        /// </summary>
        public int CurrentNodeId { get; set; }
        /// <summary>
        /// The story node data. Contains the information connected to the storynode. 
        /// </summary>
        public StoryNodeDto Node { get; set; } = new StoryNodeDto();
    }
}
