using DOTNET_PROJECT.Application.Dtos;
using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Domain.Models;
using DOTNET_PROJECT.Infrastructure.Repositories;
public class userService : IUserService
{
    private readonly IUnitOfWork unitOfWork;


    public userService(IUnitOfWork _uow)
    {
        _uow = unitOfWork; 
    }


    public async Task<UserDto> registerAccount(RegisterUserDto registerUserDto)
    {
        try
        {
            await unitOfWork.BeginAsync();

            var user = new User
            {
                Username = registerUserDto.Username,
                Password = registerUserDto.Password,

            };

            await unitOfWork.UserRepository.Create(user);

            // save changes & commit
            await unitOfWork.SaveAsync();
            await unitOfWork.CommitAsync();

            return ReturnUserDto(user);
        }
        catch ( Exception e)
        {
            // throw error and rollback transaction which disposes of it
            await unitOfWork.RollBackAsync();
            throw new Exception("Error with registering account", e);

        }
        
    }

    

    public async Task<UserDto> login(LoginUserDto loginUserDto)
    {
        try
        {
            var user = await unitOfWork.UserRepository.GetUserByUsername(loginUserDto.Username);
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




}

    