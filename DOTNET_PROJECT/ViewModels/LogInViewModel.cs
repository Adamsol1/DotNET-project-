using System.ComponentModel.DataAnnotations;


namespace DOTNET_PROJECT.Viewmodels;
/// <summary>
/// Viewmodel used for login.
/// Contains username nad password fields retrieved from a form.
/// Uses server side validation on input.
/// </summary>
public class LogInViewModel

{
    /// <summary>
    /// The username of the user.
    /// The input is required and has to be between 2 and 15 characters. 
    /// </summary>
    [Required]
    [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{2,15}", ErrorMessage = "The name must be between 2 and 20 characters or letters")]
    public string Username { get; set; }
    
    /// <summary>
    /// The password of the user.
    /// The input is required and has to be between 2 and 20 characters. 
    /// </summary>
    [Required]
    [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{2,20}", ErrorMessage = "The Name must be numbers or letters and between 2 to 20 characters.")]
    public string Password { get; set; }
}