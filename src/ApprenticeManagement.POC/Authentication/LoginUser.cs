using System.ComponentModel.DataAnnotations;

namespace ApprenticeManagement.POC.Authentication;

public class LoginUser
{
    [Required]
    public string UserName { get; set; }
}