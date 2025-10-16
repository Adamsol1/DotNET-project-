using System.ComponentModel.DataAnnotations;


namespace DOTNET_PROJECT.Viewmodels;

public class LogInViewModel
{
    [Required]
    [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{2,15}", ErrorMessage = "The name must be between 2 and 20 characters or letters")]
    public string Username { get; set; }
    
    [Required]
    [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{2,20}", ErrorMessage = "The Name must be numbers or letters and between 2 to 20 characters.")]
    public string Password { get; set; }
}