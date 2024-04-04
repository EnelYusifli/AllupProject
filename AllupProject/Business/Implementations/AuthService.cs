using Microsoft.AspNetCore.Identity;
using AllupProject.Areas.Admin.ViewModels;
using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.Models;

namespace AllupProject.Business.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }
    public async Task LoginAsync(AdminLoginViewModel adminLoginViewModel)
    {
        IdentityUser? admin = await _userManager.FindByNameAsync(adminLoginViewModel.UserName);
        if (admin is null)
            throw new EntityCannotBeFoundException("Invalid credentials");
        var result = await _signInManager.PasswordSignInAsync(admin, adminLoginViewModel.Password, false, false);
        if (!result.Succeeded)
            throw new NotSucceededException("Invalid credentials");
    }
}
