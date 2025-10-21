using DOTNET_PROJECT.Application.Dtos;
using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Infrastructure.Repositories;
using Serilog;

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
/// <summary>
/// Method used to register an account. 
/// </summary>
/// <param name="registerUserDto"></param>
/// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="loginUserDto"></param>
    /// <returns></returns>
    
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
    /// <summary>
    /// Attempts to delete account with given id. 
    /// </summary>
    /// <param name="id">Identifier of account that is being deleted</param>
    /// <returns>boolean value</returns>
    public async Task<bool> Delete(int id)
    {
        await _uow.BeginAsync();
        try
        {
            //Retrieve the user to confirm they exist
            var user = await _uow.UserRepository.GetById(id);
            if (user == null)
            {
                
                await _uow.RollBackAsync();
                _logger.LogWarning("[Userservice] User with id {Id} doesn't exist", id);
                return false;
            }
            //Delete the user and commit transaction
            await _uow.UserRepository.Delete(id);
            await _uow.CommitAsync();
            _logger.LogInformation("[Userservice] User with id {Id} deleted", id);
            return true;
        }
        catch
        {
            //Roll back changes if an error cause
            await  _uow.RollBackAsync();
            _logger.LogError($"[Userservice] User with id {id} was not deleted");
            return false;
        }
    }


    /// <summary>
    /// Updates a user's password by username
    /// </summary>
    /// <param name="updatePasswordDto">DTO with username and new password</param>
    /// <returns>The new updated dto</returns>
    public async Task<UserDto> UpdatePassword(UpdatePasswordDto updatePasswordDto)
    {
        await  _uow.BeginAsync();
        try
        {
            //Retrieve used by username
            var user = await _uow.UserRepository.GetUserByUsername(updatePasswordDto.Username);
            if (user == null)
            {
                _logger.LogWarning("[Userservice] User with username {Username} not found", updatePasswordDto.Username);
                return null;
            }
            
            //Update password field

            user.Password = updatePasswordDto.Password;
            
            //Apply the update and save
            await _uow.UserRepository.Update(user);
            await _uow.CommitAsync();
            _logger.LogInformation("[Userservice] User with username {Username} updated", user.Username);
            
            //Return the updated user
            return ReturnUserDto(user);
        }
        catch (Exception e)
        {
            //Rollback changes if error occurs
         await _uow.RollBackAsync();
         _logger.LogError(e, "[Userservice] Error updating password");
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

