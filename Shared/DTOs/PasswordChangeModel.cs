using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class PasswordChangeModel
{
    [Required(ErrorMessage = "There are empty required fields")]
    public string CurrentPassword { get; set; } = "";

    [Required(ErrorMessage = "There are empty required fields")]
    [MinLength(3, ErrorMessage = "The minimum length of the password must be at least 6 characters.")]
    public string NewPassword { get; set; } = "";

    [Required(ErrorMessage = "There are empty required fields")]
    [Compare(nameof(NewPassword), ErrorMessage = "The two passwords are different")]
    public string ConfirmPassword { get; set; } = "";
}