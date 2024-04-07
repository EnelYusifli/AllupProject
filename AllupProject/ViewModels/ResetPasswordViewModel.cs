using System.ComponentModel.DataAnnotations;

namespace AllupProject.ViewModels;

public class ResetPasswordViewModel
{
    public string Email { get; set; }
    public string Token { get; set; }
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}
