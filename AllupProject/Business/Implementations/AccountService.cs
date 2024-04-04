using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.DAL;
using AllupProject.Models;
using AllupProject.ViewModels;

namespace AllupProject.Business.Implementations;

public class AccountService : IAccountService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly AllupDbContext _context;

    public AccountService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AllupDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }
    public async Task LoginAsync(UserLoginViewModel userLoginViewModel)
    {
        IdentityUser? member = await _userManager.FindByNameAsync(userLoginViewModel.UserName);
        if (member is null)
            throw new EntityCannotBeFoundException("Invalid credentials");
        var result = await _signInManager.PasswordSignInAsync(member, userLoginViewModel.Password, false, false);
        if (!result.Succeeded)
            throw new NotSucceededException("Invalid credentials");
    }

    public async Task RegisterAsync(UserRegisterViewModel userRegisterViewModel)
    {
        IdentityUser member = new IdentityUser()
        {
            UserName = userRegisterViewModel.UserName,
            Email = userRegisterViewModel.Email
        };
        if (await _context.Users.AnyAsync(x => x.NormalizedUserName == member.UserName))
            throw new NameAlreadyExistException("UserName", "UserName is already exist");
        if (await _context.Users.AnyAsync(x => x.NormalizedEmail == member.Email))
        {
            throw new NameAlreadyExistException("Email", "Email is already exist");
        }
        var result = await _userManager.CreateAsync(member, userRegisterViewModel.Password);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
            {
                throw new NotSucceededException(err.Description);
            }
        }
        var roleResult = await _userManager.AddToRoleAsync(member, "Member");
        if (!roleResult.Succeeded)
        {
            foreach (var err in roleResult.Errors)
            {
                throw new NotSucceededException(err.Description);
            }
        }
    }
    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
