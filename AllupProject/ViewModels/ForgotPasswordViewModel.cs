using System.ComponentModel.DataAnnotations;

namespace AllupProject.ViewModels;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
