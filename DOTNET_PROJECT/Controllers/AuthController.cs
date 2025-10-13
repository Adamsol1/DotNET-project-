using Microsoft.AspNetCore.Mvc;
using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Application.Dtos;

namespace DOTNET_PROJECT.Controllers;

[ApiController] // this is a api controller
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserDto registerUserDto)
    {

    }
    
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginUserDto loginUserDto)
    {

    }


    [HttpPost("logout")]
    public async Task<ActionResult<bool>> Logout([FromBody] int userId)
    {

    }
    
    
    
}