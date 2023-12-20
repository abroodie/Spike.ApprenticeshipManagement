using System.Security.Claims;

namespace ApprenticeManagement.POC.Authentication;

public class AuthenticatedUser : ClaimsPrincipal
{
    public string UserName => Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value ?? string.Empty;

    public string EmployerAccount =>
        Claims.FirstOrDefault(c => c.Type.Equals("EmployerAccount"))?.Value ?? string.Empty;

    public string EmployerName => Claims.FirstOrDefault(c => c.Type.Equals("EmployerName"))?.Value ?? string.Empty;

    public AuthenticatedUser(ClaimsIdentity identity): base(identity){}
    public AuthenticatedUser() : base() { }
}