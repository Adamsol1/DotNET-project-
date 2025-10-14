using DOTNET_PROJECT.Application.Dtos;
using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Infrastructure.Repositories;

namespace DOTNET_PROJECT.Application;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;


    public UserService(IUnitOfWork uow)
    {
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
            throw new Exception("Error with registering account", e);

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
                return null;
            }

            return ReturnUserDto(user);  
        }
        catch (Exception e)
        {
            throw new Exception("Failed to login", e);
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
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            return ReturnUserDto(user);
        }
        catch (Exception e)
        {
            throw new Exception("Error fetching user by ID", e);
        }
    }


}

    