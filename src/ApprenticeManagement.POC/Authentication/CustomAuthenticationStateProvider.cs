using System.Security.Claims;
using ApprenticeManagement.POC.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Plugin.Firebase.CloudMessaging;

namespace ApprenticeManagement.POC.Authentication;

public class CustomAuthenticationStateProvider: AuthenticationStateProvider
{
    private readonly DeviceManagementServiceClient deviceManagementService;
    public ClaimsPrincipal CurrentUser = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(DeviceManagementServiceClient deviceManagementService)
    {
        this.deviceManagementService = deviceManagementService;
    }

    public async Task Login(AuthenticatedUser user)
    {
        CurrentUser = user;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        //TODO: should have event handler on AuthenticationStateChanged event
        await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
        var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            this.deviceManagementService.RegisterDevice(
                user.UserName,
                user.EmployerAccount,
                token);
    }

    public async Task Logout()
    {
        CurrentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(CurrentUser));
    }
}