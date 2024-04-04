using AllupProject.ViewModels;

namespace AllupProject.Business.Interfaces;

public interface IAccountService
{
    Task RegisterAsync(UserRegisterViewModel userRegisterViewModel);
    Task LoginAsync(UserLoginViewModel userLoginViewModel);
    Task LogoutAsync();
}
