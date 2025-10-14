// ViewModel for starting a new game.
// Used to capture the player's name input on the start page before the game begins.
// The [Required] and [Display] attributes ensure that the field is validated
// and displayed properly in Razor forms.
using System.ComponentModel.DataAnnotations;

namespace DOTNET_PROJECT.Models
{
    public class StartGameViewModel
    {
        [Required]
        [Display(Name = "Your name")]
        public string Name { get; set; } = string.Empty;
    }
}
