using System.ComponentModel.DataAnnotations;

namespace AllupProject.Areas.Admin.ViewModels;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
