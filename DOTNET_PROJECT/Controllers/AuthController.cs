using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using DOTNET_PROJECT.Application.Interfaces.Repositories;
using DOTNET_PROJECT.Application.Interfaces.Services;
using DOTNET_PROJECT.Application.Dtos;
using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Viewmodels;
using Microsoft.AspNetCore.Authorization;
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

    public AuthController(
        IUserService userService, 
        ILogger<AuthController> logger,
        UserManager<AuthUser> userManager,
        SignInManager<AuthUser> signInManager,
        IConfiguration configuration)
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
            var user = new AuthUser
            {
                UserName = request.Username,

            };
            
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("[AuthController] Unable to create account with {@Username}", request.Username);
                return BadRequest(result.Errors);
                
            }
            
            _logger.LogInformation("[AuthController] Succesfully created account for {$Username}", request.Username);
            var gameUser = _userService.RegisterAccount(request);
            if (gameUser == null)
            {
                _logger.LogWarning("[AUTHDATABASE]The database will not be syncecd and will create error");
                return BadRequest("The database will not be syncecd and will create error");
            }
            
            return Ok("User created account successfully with ");
            //Await the answer given by the service layer. 
            
        // Todo : håndter mer spesifikke error handlings.    
        // Catch block for handling unexpected error
        } catch(Exception e)
        {
            // log the error
            _logger.LogError(e, "[AuthController] Unexpected error occured while trying to create  account for {$Username}", request.Username);
            // return the error
            return BadRequest("Unexpected error occured while creating account.");
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

            if (userDto != null && await _userManager.CheckPasswordAsync(userDto, request.Password))
            {
                _logger.LogInformation("[AuthController] Login attempt authorized for user : {@LoginUserDto}", request);
                var token =  GenerateJwtToken(userDto);

                var user = await _userService.Login(request);
                
                return Ok(new { Token = token, userId = user.Id, username = user.Username });
            }
            _logger.LogWarning("[AuthController] Login attempt failed for user : {@LoginUserDto}", request);
            return Unauthorized();
            
        }
            //Todo : Håndet mer konkret error handling.
            catch (Exception e)
            {
                // log the error
                _logger.LogError(e, "[AuthController] Unexpected error occured while trying to login.");
                // return the error
                return BadRequest("Unexpected error occured while trying to login.");
            }
        }
    


    /// <summary>
        /// Post method for logout. 
        /// </summary>
        /// <returns>Success message</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        
        await _signInManager.SignOutAsync();
        _logger.LogInformation("[AuthController] Logged out.");
        return Ok("Logged out successfully");

    }
    
    /// <summary>
    /// Get method for getting current user info
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("user")]
    [Authorize]
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

  

    
}