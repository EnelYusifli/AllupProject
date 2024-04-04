using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using AllupProject.DAL;
using AllupProject.Areas.Admin.ViewModels;
using AllupProject.CustomExceptions.Common;
using AllupProject.Business.Interfaces;
using AllupProject.Models;

namespace AllupProject.Areas.Admin.Controllers;
[Area("Admin")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly AllupDbContext _context;

    public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, AllupDbContext context, IAuthService authService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _context = context;
        _authService = authService;
    }
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginViewModel adminLoginViewModel)
    {
        if (!ModelState.IsValid) return View();
        try
        {
            await _authService.LoginAsync(adminLoginViewModel);
        }
        catch (EntityCannotBeFoundException ex)
        {
            ModelState.AddModelError("",ex.Message);
            return View();
        } 
        catch (NotSucceededException ex)
        {
            ModelState.AddModelError("",ex.Message);
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("",ex.Message);
            return View();
        }
        return RedirectToAction("Index","Dashboard");
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordVM)
    {
        if (!ModelState.IsValid) return View();
        var user = await _userManager.FindByEmailAsync(forgotPasswordVM.Email);

        if (user is not null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var link = Url.Action("ResetPassword", "Auth", new { email = forgotPasswordVM.Email, token = token }, Request.Scheme);


            //MailAddress to = new MailAddress(forgotPasswordVM.Email);
            //MailAddress from = new MailAddress("yusiflienel@gmail.com");

            //MailMessage email = new MailMessage(from, to);
            //email.Subject = "Testing out email sending";
            //email.Body = link;

            //SmtpClient smtp = new SmtpClient();
            //smtp.Host = "smtp.gmail.com";
            //smtp.Port = 25;
            //smtp.Credentials = new NetworkCredential("smtp_username", "smtp_password");
            //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //smtp.EnableSsl = true;
            //smtp.Send(email);

            return View("InfoPage");
        }
        else
        {
            ModelState.AddModelError("Email", "User not found");
            return View();
        }
    }

    public IActionResult ResetPassword(string email,string token)
    {
        if(email == null || token == null) return NotFound();
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordVM)
    {
        if (!ModelState.IsValid)return View(); 
        var user = await _userManager.FindByEmailAsync(resetPasswordVM.Email);
        if (user is not null)
        {
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                    return View();
                }
            }
        }
        else return NotFound();

        return RedirectToAction(nameof(Login));
    }


    //public async Task<IActionResult> CreateAdmin()
    //{
    //    IdentityUser superAdmin = new IdentityUser()
    //    {
    //        UserName = "SuperAdmin",
    //        Email = "superadmin@gmail.com"
    //    };
    //    var result = await _userManager.CreateAsync(superAdmin, "SuperAdmin123!");
    //    IdentityUser admin = new IdentityUser()
    //    {
    //        UserName = "admin",
    //        Email = "admin@gmail.com"
    //    };
    //    var result2 = await _userManager.CreateAsync(admin, "Admin123!");

    //    return Ok(result);
    //}
    //public async Task<IActionResult> CreateRole()
    //{
    //    IdentityRole superAdminRole = new IdentityRole("SuperAdmin");
    //    IdentityRole adminRole = new IdentityRole("Admin");
    //    IdentityRole memberRole = new IdentityRole("Member");
    //    await _roleManager.CreateAsync(superAdminRole);
    //    await _roleManager.CreateAsync(adminRole);
    //    await _roleManager.CreateAsync(memberRole);
    //    return Ok();
    //}
    //public async Task<IActionResult> AddRole()
    //{
    //    IdentityUser superAdmin = await _userManager.FindByNameAsync("SuperAdmin");
    //    IdentityUser admin = await _userManager.FindByNameAsync("Admin");
    //    var result = await _userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
    //    var result2 = await _userManager.AddToRoleAsync(admin, "Admin");
    //    return Ok(result);
    //}
}
