using System.Security.Claims;

namespace DASApprenticeManagementPOC.Authentication;

public class UserAccountAuthenticator
{
    private readonly UserAccounts userAccounts;

    public UserAccountAuthenticator(UserAccounts userAccounts)
    {
        this.userAccounts = userAccounts;
    }

    public (bool IsValid, ClaimsPrincipal AuthenticatedUser) AuthenticateUser(string userName)
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
        var userAccount =
            userAccounts.Users.FirstOrDefault(u => u.Username.Equals(userName, StringComparison.OrdinalIgnoreCase));
        if (userAccount == null) return (false, authenticatedUser);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userAccount.Name),
            new Claim(ClaimTypes.Email, userAccount.Username),
            new Claim(ClaimTypes.Role,"Employer"),
            new Claim("EmployerAccount",userAccount.EmployerAccount)
        };
        return (true, new ClaimsPrincipal(new ClaimsIdentity(claims, "Server authentication")));
    }
}