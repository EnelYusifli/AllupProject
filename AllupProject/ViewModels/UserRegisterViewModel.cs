using System.ComponentModel.DataAnnotations;

namespace AllupProject.ViewModels;

public class UserRegisterViewModel
{
    [DataType(DataType.Text)]
    public string UserName { get; set; }
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}
