using AllupProject.Business.Interfaces;
using AllupProject.CustomExceptions.Common;
using AllupProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AllupProject.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accService;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(IAccountService accService,UserManager<IdentityUser> userManager)
    {
        _accService = accService;
        _userManager = userManager;
    }
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegisterViewModel userRegisterViewModel)
    {
        if (!ModelState.IsValid)return View();
        try
        {
            await _accService.RegisterAsync(userRegisterViewModel);
        }
        catch (NameAlreadyExistException ex)
        {
            ModelState.AddModelError(ex.PropertyName,ex.Message);
            return View();
        }
        catch (NotSucceededException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("",ex.Message);
            return View();
        }
            return RedirectToAction("index","home");
    }

    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginViewModel userLoginViewModel)
    {
        if (!ModelState.IsValid) return View();
        try
        {
            await _accService.LoginAsync(userLoginViewModel);
        }
        catch (EntityCannotBeFoundException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        } 
        catch (NotSucceededException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
       await _accService.LogoutAsync();

        return RedirectToAction("Login", "Account");
    }
    public IActionResult ForgotPassword()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(AllupProject.ViewModels.ForgotPasswordViewModel forgotPasswordVM)
    {
        if (!ModelState.IsValid) return View();
        var user = await _userManager.FindByEmailAsync(forgotPasswordVM.Email);

        if (user is not null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var link = Url.Action("ResetPassword", "Account", new { email = forgotPasswordVM.Email, token = token }, Request.Scheme);


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

    public IActionResult ResetPassword(string email, string token)
    {
        if (email == null || token == null) return NotFound();
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(AllupProject.ViewModels.ResetPasswordViewModel resetPasswordVM)
    {
        if (!ModelState.IsValid) return View();
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

}
