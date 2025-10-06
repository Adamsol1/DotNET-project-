 using DOTNET_PROJECT.Threading.Tasks;
using DOTNET_PROJECT.Collections.Generic;

namespace DOTNET_PROJECT.Application.Interfaces;

public interface IUserService
{

    /*
    These are the methods I belive we would need for the UserService.
    Adam can add more if he wants. or tell me what he needs for auth, like session management etc.

    RegisterAccount() -> registers a new user asynchronously from input.
    Login() -> logs in a user and creates a session.
    Logout() -> logs out a user and destroys the session.
    ChangePassword() -> changes a user's password.

    // admin methods ?
    GetAllUsers() -> gets all users.
    GetUserById() -> gets a user by id.
    checkUserRole() -> checks a user's role.
    UpdateUser() -> updates a user.
    DeleteUser() -> deletes a user.
    */

    Task<UserDto> RegisterAccount(RegisterUserDto registerUserDto);
    Task<UserDto> Login(LoginUserDto loginUserDto);
    Task<UserDto> Logout();
    Task<UserDto> ChangePassword(ChangePasswordDto changePasswordDto);
}