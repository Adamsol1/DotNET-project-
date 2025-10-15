using System.ComponentModel.DataAnnotations;


namespace DOTNET_PROJECT.Viewmodels;

public class LogInViewModel
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}