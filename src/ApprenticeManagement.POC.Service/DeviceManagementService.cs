using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;

namespace ApprenticeManagement.POC.Service;

public class DeviceManagementService
{
    private static NotificationHubClient notificationHub = NotificationHubClient.CreateClientFromConnectionString(Environment.GetEnvironmentVariable("NotificationHub"), Environment.GetEnvironmentVariable("HubName"));

    private List<DeviceUser> usersData = new List<DeviceUser>();

    private static async Task RemoveClients(string token, ILogger logger)
    {
        try
        {
            var descriptions = await notificationHub.GetRegistrationsByChannelAsync(token, 100);
            logger.LogInformation($"Found {descriptions.Count()} for token: {token}");
            foreach (var registrationDescription in descriptions)
            {
                logger.LogInformation($"Deleting registration: {registrationDescription.RegistrationId} for token: {token}");
                await notificationHub.DeleteRegistrationAsync(registrationDescription);
            }
            logger.LogInformation($"Finished deleting registrations for client: {token}");
        }
        catch (Exception e)
        {
            logger.LogError($"Error adding registration: {e.Message}", e);
            throw;
        }

    }

    public async Task Register(string employer, string user, string deviceId, ILogger logger)
    {
        //await RemoveClients(deviceId, logger);
        usersData.RemoveAll(user => user.DeviceId.Equals(deviceId));
        var registration = new FcmRegistrationDescription(deviceId)
        {
            RegistrationId = await notificationHub.CreateRegistrationIdAsync(),
            Tags = new HashSet<string>
            {
                $"user_{user}",
                $"employer_{employer}"
            }
        };

        //await notificationHub.CreateOrUpdateRegistrationAsync(registration);
        usersData.Add(new DeviceUser { DeviceId = deviceId, Employer = employer, UserName = user });
    }

    public List<DeviceUser> GetAllUsers()
    {
        return usersData;
    }
}