using System.Diagnostics;
using System.Net.Http.Json;

namespace ApprenticeManagement.POC.Common;

public class DeviceManagementServiceClient
{
    public HttpClient HttpClient { get; set; }
    public DeviceManagementServiceClient(string apprenticeManagementServiceBaseUri)
    {
        if (apprenticeManagementServiceBaseUri == null) throw new ArgumentNullException(nameof(apprenticeManagementServiceBaseUri));
        HttpClient = new HttpClient()
        {
            BaseAddress = new Uri(apprenticeManagementServiceBaseUri)
        };
    }

    public async Task RegisterDevice(string userName, string employer, string deviceToken)
    {
        try
        {
            var response = await HttpClient.PostAsync($"employers/{employer}/users/{userName}/devices/{deviceToken}", new StringContent(string.Empty));
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Registered device: user: {userName}, employer: {employer}, device: {deviceToken}");
            }
            else
                Debug.WriteLine($"Failed to register user: {userName}, employer: {employer}, device: {deviceToken}, Response: {response.StatusCode}");

        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error registering device: {e}");
        }
    }

    public async Task<List<DeviceUser>> GetDeviceUsers()
    {
        try
        {
            var devices = await HttpClient.GetFromJsonAsync<DeviceUsersPayload>("devices");
            return devices?.DeviceUsers ?? new List<DeviceUser>();
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error getting devices: {e}");
            throw e;
        }
    }

    public async Task NotifyAll(string message)
    {
        try
        {
            await HttpClient.PostAsync($"notify/all/{message}", new StringContent(string.Empty));
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error getting devices: {e}");
            throw e;
        }
    }

    public async Task NotifyEmployers(string employer, string message)
    {
        try
        {
            await HttpClient.PostAsync($"notify/employer/{employer}/{message}", new StringContent(message));
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error getting devices: {e}");
            throw e;
        }
    }

    public async Task NotifyUsers(string user, string message)
    {
        try
        {
            await HttpClient.PostAsync($"notify/user/{user}/{message}", new StringContent(message));
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error getting devices: {e}");
            throw e;
        }
    }

}