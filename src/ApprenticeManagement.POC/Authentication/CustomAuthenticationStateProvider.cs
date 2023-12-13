using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ApprenticeManagement.POC.Authentication;

public class CustomAuthenticationStateProvider: AuthenticationStateProvider
{
    private ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());
    public CustomAuthenticationStateProvider()
    {

    }

    public async Task Login(ClaimsPrincipal user)
    {
        currentUser = user;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Logout()
    {
        currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(currentUser));
    }
}