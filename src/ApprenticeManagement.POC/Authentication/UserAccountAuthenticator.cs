using System.Security.Claims;

namespace ApprenticeManagement.POC.Authentication;

public class UserAccountAuthenticator
{
    private readonly UserAccounts userAccounts;

    public UserAccountAuthenticator(UserAccounts userAccounts)
    {
        this.userAccounts = userAccounts;
    }

    public (bool IsValid, AuthenticatedUser AuthenticatedUser) AuthenticateUser(string userName)
    {
        var authenticatedUser = new AuthenticatedUser(new ClaimsIdentity());
        var userAccount =
            userAccounts.Users.FirstOrDefault(u => u.Username.Equals(userName, StringComparison.OrdinalIgnoreCase));
        if (userAccount == null) return (false, authenticatedUser);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userAccount.Name),
            new Claim(ClaimTypes.Email, userAccount.Username),
            new Claim(ClaimTypes.Role,"Employer"),
            new Claim("EmployerAccount",userAccount.EmployerAccount),
            new Claim("EmployerName",userAccount.EmployerName)
        };
        return (true, new AuthenticatedUser(new ClaimsIdentity(claims, "Server authentication")));
    }
}