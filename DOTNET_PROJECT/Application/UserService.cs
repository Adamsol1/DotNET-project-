using DOTNET_PROJECT.Application.Dtos;
using DOTNET_PROJECT.Application.Interfaces.Repositories;
using DOTNET_PROJECT.Application.Interfaces.Services;
using DOTNET_PROJECT.Domain.Models;


namespace DOTNET_PROJECT.Application;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork uow, ILogger<UserService> logger)
    {
        _logger = logger;
        _uow = uow; 
    }

    public async Task<UserDto> RegisterAccount(RegisterUserDto registerUserDto)
    {
        try{

            await _uow.BeginAsync();

            var user = new User
            {
                Username = registerUserDto.Username,
                Password = registerUserDto.Password,

            };

            await _uow.UserRepository.Create(user);

            // save changes & commit
            await _uow.SaveAsync();
            await _uow.CommitAsync();

            return ReturnUserDto(user);
        }
        catch ( Exception e)
        {
            // throw error and rollback transaction which disposes of it
            await _uow.RollBackAsync();
            _logger.LogError(e, "[Userservice] Error registering account");
            throw;
        }
        
    }
    
    public async Task<UserDto> Login(LoginUserDto loginUserDto)
    {
        try
        {
            var user = await _uow.UserRepository.GetUserByUsername(loginUserDto.Username);
            //Check if user exists
            if(user == null)
            {
                _logger.LogWarning("[Userservice] User with username {Username} doesn't exist", loginUserDto.Username);
                return null;
            }

            return ReturnUserDto(user);  
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Userservice] Error during login for username {Username}", loginUserDto.Username);
            throw;
        }
        
    }

    private static UserDto ReturnUserDto(User user) => new UserDto
        {
            Id = user.Id,
            Username = user.Username
        };

    public async Task<UserDto> GetUserById(int id)
    {
        try
        {
            var user = await _uow.UserRepository.GetById(id);

            if (user == null)
            {
                _logger.LogWarning("[Userservice] User with id {id} not found", id);
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            return ReturnUserDto(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Userservice] Error fetching user with id {id}", id);
            throw;
        }
    }
}

    