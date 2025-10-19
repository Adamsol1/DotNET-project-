using Microsoft.AspNetCore.Mvc;
using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Application.Dtos;
using DOTNET_PROJECT.Viewmodels;

namespace DOTNET_PROJECT.Controllers;


[Route("Auth")]
public class AuthController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }
    
    
    /// <summary>
    /// This controller method will contact the service layer about registering a user with given information.
    /// If sucessfull the method will inform the user about the account registraion.
    /// If unsuccesfull the method will return an error to the user explaining why it failed. 
    /// </summary>
    /// <param name="registerUserDto"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
        // Attempt to contact service layer about the account registration
        try
        {
            //Await the answer given by the service layer. 
            var userDto = await _userService.RegisterAccount(registerUserDto);
            //If the service layer was unable to create the account inform the user. 
            if (userDto == null)
            {
                _logger.LogWarning("[AuthController] Unable to create account");
                return View(registerUserDto);
            }
            //If account registration succesfull the user will be informed and redirected to the login page. 
            // TODO : Inform user about account registration
            return RedirectToAction("Login", "Auth");
        //If unable to contact service layer error will be given
        } catch(Exception e)
        {
            _logger.LogError(e, "[AuthController] Error contacting service layer");
            return View(registerUserDto);
        }
    }


    /// <summary>
    /// Method for the controller to contact the service layer to check if login attempt is valid.
    /// If the login attempt is valid the user will be redirected to the home page of the website.
    /// If not valid the user will be given an error explaining why they were unable to login. 
    /// </summary>
    /// <param name="loginUserDto"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpPost("login")]
    public async Task<IActionResult> Login( LogInViewModel vm)
    {
        if (!ModelState.IsValid)
                return View(vm);
        // Try to use user service for login
        try
        {
            var dto = new LoginUserDto { Username = vm.Username, Password = vm.Password };

            var userDto = await _userService.Login(dto);

            //Checks if the user service found a user with given username and password
            if (userDto == null)
            {
                _logger.LogWarning("[AuthController] Username for user {Username} not found", vm.Username);
                return View(vm);
            }

            // If the service returns a valid userDTO the webpage will navigate to the Home page. 
            return RedirectToAction("index", "home");
        }
            //If unable to use the service the user will be given an error message. 
            catch (Exception e)
            {
                _logger.LogError(e, "[AuthController] Error occurred during login for user {Username}", vm.Username);
                return View(vm);
            }
        }
    


    /// <summary>
        /// Get method for logout. 
        /// </summary>
        /// <returns>The view of the loginpage</returns>
    public IActionResult Logout()
    {
        return RedirectToAction("Login");
    }
    
    /// <summary>
    /// Get method for displaying the login page
    /// </summary>
    /// <returns>The view of the loginpage</returns>
    [HttpGet("login")]
    public IActionResult login()
    {
        return View(new LogInViewModel());
    }

    ///<summary
    /// Get method for displaying the register page
    /// </summary>
    [HttpGet("register")]
    public IActionResult Register()
    {
        return View();
    }
    
    
}