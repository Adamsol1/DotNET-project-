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
    public async Task<IActionResult> Register(LogInViewModel vm)
    {

        if (!ModelState.IsValid)
        {
            _logger.LogError("[AuthController] User input did not pass validation");
            return View(vm);
        }
            
        // Attempt to contact service layer about the account registration
        try
        {
            var registerUserDto = new RegisterUserDto{Username = vm.Username, Password = vm.Password};
            //Await the answer given by the service layer. 
            var userDto = await _userService.RegisterAccount(registerUserDto);
            //If the service layer was unable to create the account inform the user. 
            if (userDto == null)
            {
                // TODO : Inform user about the error
                _logger.LogError("[AuthController] Error registering account!");
                return View(vm);
            
            }
            //If account registration succesfull the user will be informed and redirected to the login page. 
            // TODO : Inform user about account registration
            _logger.LogInformation("[AuthController] Account successfully created!");
            
            return RedirectToAction("Login", "Auth");
        //If unable to contact service layer error will be given
        } catch(Exception e)
        {
            // TODO : Log error
            _logger.LogError(e, "[AuthController] Unable to contact service layer");
            return View(vm);
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
        {
            _logger.LogError("[AuthController] User input did not pass validation");
            return View(vm);
        }

        // Try to use user service for login
        try
        {
            //Makes a new dto used for transer between layers and contacts service layer for a login with given credentials. 
            var dto = new LoginUserDto { Username = vm.Username, Password = vm.Password };
            var userDto = await _userService.Login(dto);

            //Checks if the user service found a user with given username and password
            if (userDto == null)
            {
                ModelState.AddModelError(nameof(vm.Username), "Wrong username or password");
                _logger.LogError("[AuthController] Error logging in! Invalid username or password");
                return View(vm);
            }

            // If the service returns a valid userDTO the webpage will navigate to the Home page. 
            _logger.LogInformation("[AuthController] Account successfully logged in!");
            //Sets a session id that is used when deleting an account. The id matches the user id. 
            HttpContext.Session.SetInt32("UserId", userDto.Id);
            return RedirectToAction("index", "home");
        }
            //If unable to use the service the user will be given an error message. 
            catch (Exception e)
            {
                // TODO : Legge til korrekt feilmelding
                _logger.LogError(e, "[AuthController] Error logging in!");
                return View(vm);
            }
        }
    /// <summary>
    /// Method for updating password on given user.
    /// If the password is changed the user will be redirected to the login page where they now can use their new credentials.
    /// If the password change is failed the page will be updated with an error message. 
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    [HttpPost("updatepassword")]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("[AuthController] User input did not pass validation");
            return View(vm);
        }

        // Try to use user service for Update password
        try
        {
            //Creates the updated user information
            var dto = new UpdatePasswordDto() { Username = vm.Username, Password = vm.Password };
            
            //Checks if there is a given account with username
            var userDto = await _userService.UpdatePassword(dto);

            //Checks if the user service found a user with given username and password
            if (userDto == null)
            {
                //If no user with given credentials inform the user and return the view. 
                ModelState.AddModelError(nameof(vm.Username), "User does not exist");
                _logger.LogError("[AuthController] Error logging in!");
                return View(vm);
            }
            
            _logger.LogInformation("[AuthController] Account successfully updated!");

            // If the service returns a valid userDTO the webpage will navigate to the Home page. 
           
            return RedirectToAction("Login", "Auth");
        }
        //If unable to use the service the user will be given an error message. 
        catch (Exception e)
        {
            // TODO : Legge til korrekt feilmelding
            _logger.LogError(e, "[AuthController] Error logging in!");
            return View(vm);
        }
        
    }
    
    /// <summary>
    /// Delete method used to delete account. The method will use the session id to be able to retrieve the correct user from the database.
    /// If successful the user is logged out.
    /// If unsuccessful the user will be informed. 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpPost("Delete")]
    public async Task<IActionResult> Delete()
    {
        //Tries to contact the service layer
        try
        {
            //Retrieves the id of the current session/user
            var userId = HttpContext.Session.GetInt32("UserId")
                         ?? throw new Exception("UserId not found in current session");
            //Contacts user service for the delete with given userid
            bool delete = await _userService.Delete(userId);
            if (delete == false)
            {
                //Informs the user and loggs error
                TempData["Error"] = "Deletion of account failed";
                _logger.LogError("[AuthController] Error deleting account!");
                
            }
            
            //Logout if account is deleted
            _logger.LogInformation("[AuthController] Account successfully deleted!");
            return RedirectToAction("Login", "Auth");
        }
        //Error if unable to contact service layer
        catch (Exception e)
        {
            _logger.LogError(e, "[AuthController] Error deleting account!");
            return  RedirectToAction("Index", "Home");


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
        return View(new LogInViewModel());
    }
    
    /// <summary>
    /// Get method for displaying the update page
    /// </summary>
    
    [HttpGet("updatepassword")]
    public IActionResult UpdatePassword()
    {
        return View(new UpdatePasswordViewModel());
    }
    
}