using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using DOTNET_PROJECT.Application.Interfaces.Repositories;
using DOTNET_PROJECT.Application.Interfaces.Services;
using DOTNET_PROJECT.Application.Dtos;
using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Viewmodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace DOTNET_PROJECT.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;
    private readonly UserManager<AuthUser> _userManager;
    private readonly SignInManager<AuthUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
        _configuration = configuration;
        _userManager = userManager;
        _configuration = configuration;
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
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserDto request)
    {
        // Attempt to contact service layer about the account registration
        try
        {
            //Await the answer given by the service layer. 
            var userDto = await _userService.RegisterAccount(request);
            //If the service layer was unable to create the account inform the user. 
            if (userDto == null)
            {
                // TODO : Inform user about the error
                return BadRequest("Failed to create account. Username may already exist.");
                
            }

            //If account registration succesfull the user will be informed and redirected to the login page. 
            // TODO : Inform user about account registration
            return Ok(userDto);

        //If unable to contact service layer error will be given
        } catch(Exception e)
        {
            // log the error
            _logger.LogError(e, "[AuthController] Failed to create account. Username may already exist.");
            // return the error
            return BadRequest("Failed to create account. Username may already exist.");
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
    public async Task<ActionResult<LoginUserDto>> Login([FromBody] LoginUserDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        // Try to use user service for login
        try
        {
            
            var userDto = await _userManager.FindByNameAsync(request.Username);

            //Checks if the user service found a user with given username and password
            if (userDto == null)
            {
                // return the error
                _logger.LogWarning($"User {request.Username} user not authorized for username {request.Username}.");
                return Unauthorized("Invalid username or password.");
            }

            // If the service returns a valid userDTO the webpage will navigate to the Home page. 
            _logger.LogInformation("[AuthController] Authorized login for user {@LoginUserDto}", request);
            var token = GenerateJwtToken(userDto);
            return Ok(new{Token = token});
        }
            //If unable to use the service the user will be given an error message. 
            catch (Exception e)
            {
                // log the error
                _logger.LogError(e, "[AuthController] Failed to login. Invalid username or password.");
                // return the error
                return BadRequest("Failed to login. Invalid username or password.");
            }
        }
    


    /// <summary>
        /// Post method for logout. 
        /// </summary>
        /// <returns>Success message</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // return the success message since we dont use JWT tokens yet
        await _signInManager.SignOutAsync();
        _logger.LogInformation("[AuthController] Logged out.");
        return Ok(new { message = "Logged out successfully" });

    }
    
    /// <summary>
    /// Get method for getting current user info
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("user")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        // For now, return the admin user since we're using admin credentials
        // In JWT implementation, this would extract user from token
        return Ok(new UserDto 
        { 
            Id = 1, 
            Username = "admin", 
        });
    }


    private string GenerateJwtToken(AuthUser user)
    {
        var JWTKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(JWTKey))
        {
            _logger.LogWarning($"JWT Key not set. Key: {JWTKey}");
            throw new InvalidOperationException("JWT Key not set. Key: {JWTKey}");
        }
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
        };
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );
        _logger.LogInformation("[AuthController] Generated JWT token for user @{UserName}.", user.UserName);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // COMMENTED OUT FOR REACT FRONTEND - These were MVC view methods
    /*
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
    */

    
}