using System.Diagnostics;

namespace ApprenticeManagement.POC;

public class DeviceManagementService
{
    public HttpClient HttpClient { get; set; }
    public DeviceManagementService(string apprenticeManagementServiceBaseUri)
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
}