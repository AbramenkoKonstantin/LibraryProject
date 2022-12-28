using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.User;

public class LoginModel
{
    [EmailAddress]
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}