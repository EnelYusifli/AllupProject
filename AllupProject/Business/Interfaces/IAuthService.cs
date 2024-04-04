using AllupProject.Areas.Admin.ViewModels;

namespace AllupProject.Business.Interfaces;

public interface IAuthService
{
    Task LoginAsync(AdminLoginViewModel adminLoginViewModel);
}
