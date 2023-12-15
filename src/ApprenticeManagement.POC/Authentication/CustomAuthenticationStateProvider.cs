using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Plugin.Firebase.CloudMessaging;

namespace ApprenticeManagement.POC.Authentication;

public class CustomAuthenticationStateProvider: AuthenticationStateProvider
{
    private readonly DeviceManagementService deviceManagementService;
    private ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());
    public CustomAuthenticationStateProvider(DeviceManagementService deviceManagementService)
    {
        this.deviceManagementService = deviceManagementService;
    }

    public async Task Login(ClaimsPrincipal user)
    {
        currentUser = user;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        //TODO: should have event handler on AuthenticationStateChanged event
        await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
        var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            this.deviceManagementService.RegisterDevice(
                currentUser.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value ?? string.Empty,
                currentUser.Claims.FirstOrDefault(c => c.Type.Equals("EmployerAccount"))?.Value ?? string.Empty,
                token);
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