using System.ComponentModel.DataAnnotations;

namespace DASApprenticeManagementPOC.Authentication;

public class LoginUser
{
    [Required]
    public string UserName { get; set; }
}